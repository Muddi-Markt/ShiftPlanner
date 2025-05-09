using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Muddi.ShiftPlanner.Client.Components;
using Muddi.ShiftPlanner.Client.Configuration;
using Muddi.ShiftPlanner.Client.Entities;
using Muddi.ShiftPlanner.Client.Services;
using Muddi.ShiftPlanner.Client.Shared;
using Muddi.ShiftPlanner.Shared;
using Muddi.ShiftPlanner.Shared.Contracts.v1;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses;
using Muddi.ShiftPlanner.Shared.Entities;
using Radzen;
using Radzen.Blazor;
using Radzen.Blazor.Rendering;
using Appointment = Muddi.ShiftPlanner.Client.Entities.Appointment;

namespace Muddi.ShiftPlanner.Client.Pages.Locations;

public partial class LocationPage
{
	[Inject] private IOptions<AppCustomization> AppCustomization { get; init; } = default!;

	private string RadzenStyle
		=> $"height: {CaclculateHeight()}px !important;";

	private string CaclculateHeight()
	{
		var hours = (AppCustomization.Value.EndTimeSpan - AppCustomization.Value.StartTimeSpan).TotalHours;
		Console.WriteLine("end: " + AppCustomization.Value.EndTimeSpan);
		Console.WriteLine("start: " + AppCustomization.Value.StartTimeSpan);
		Console.WriteLine("hours: " + hours);
		return (48.125 * hours + 88.75).ToInvariantString();
	}

	[Inject] private DialogService DialogService { get; set; } = default!;
	[Inject] private ShiftService ShiftService { get; set; } = default!;
	[Inject] private ILogger<LocationPage> Logger { get; set; } = default!;
	[Inject] private IJSRuntime JsRuntime { get; set; } = default!;

	/// <summary>
	/// LocationId
	/// </summary>
	[Parameter]
	public Guid Id { get; set; }

	[SupplyParameterFromQuery] [Parameter] public bool ShowOnlyUsersShifts { get; set; }
	[SupplyParameterFromQuery] [Parameter] public int SelectedViewIndex { get; set; }

	public int GetSelectedViewIndex() => _scheduler is null
		? -1
		: _scheduler.IsSelected(_dayView)
			? DayViewIndex
			: _scheduler.IsSelected(_weekView)
				? WeekViewIndex
				: -1;

	[SupplyParameterFromQuery]
	[Parameter]
	public DateTime StartDate
	{
		get => _startDate;
		set => _startDate = value == default ? _startDate : value;
	}


	[CascadingParameter] public Task<AuthenticationState> AuthenticationState { get; set; }
	[CascadingParameter] public MainLayout MainLayout { get; set; } = default!;

	private ShiftLocation? _location;
	private ClaimsPrincipal? _user;

	private IEnumerable<Appointment> Shifts => ShowOnlyUsersShifts
		? _shifts.Where(s => (s as DayAppointment)?.Shift?.User.KeycloakId == _userKeycloakId)
		: _shifts;


	private RadzenScheduler<Appointment>? _scheduler;
	private IList<Appointment> _shifts = new List<Appointment>();
	private Guid _userKeycloakId;

	protected override async Task OnParametersSetAsync()
	{
		if (_location?.Id == Id)
			return;
		try
		{
			var state = await AuthenticationState;
			_location = await ShiftService.GetLocationsByIdAsync(Id);
			if (_location is null)
				throw new NullReferenceException($"Location with id {Id} not found");
			var firstAvailableShift =
				(await ShiftService.GetAllAvailableShiftTypesFromLocationAsync(Id, ShiftService.CurrentSeason.StartDate,
					ShiftService.CurrentSeason.EndDate, 1)).ToList();
			MainLayout.SetTitle($"{_location.Name} ({_location.AssignedShifts}/{_location.TotalShifts} Schichten)");
			_user = state.User;

			var firstShiftStartDate = firstAvailableShift.FirstOrDefault()?.Start;
			if (_startDate == default || _startDate < firstShiftStartDate)
			{
				var startDate =
					(firstShiftStartDate ?? ShiftService.CurrentSeason.StartDate).ToLocalTime();
				_startDate = DateTime.Now > startDate ? DateTime.Now : startDate;
			}

			_userKeycloakId = _user.GetKeycloakId();
			_isAdmin = _user.IsInRole(ApiRoles.Admin);
			await SetShifts(SelectedViewIndex);
		}
		catch (Exception ex)
		{
			if (ex is AccessTokenNotAvailableException)
				throw;
			await DialogService.Error(ex, "Error while setting parameter of places page");
		}
	}

	Task ForceReloadScheduler()
	{
		_shifts.Clear();
		return _scheduler?.Reload() ?? Task.CompletedTask;
	}


	private Task OnSlotSelect(SchedulerSlotSelectEventArgs args)
	{
		return OnSlotSelect(args.Start);
	}

	private async Task OnSlotSelect(DateTime startTime, ShiftType? type = null)
	{
		if (_location is null || _user is null)
			return;
		if (type is { OnlyAssignableByAdmin: true } && !_isAdmin)
		{
			await DialogService.Confirm($"Leider dürfen nur Admins '{type.Name}' Nutzer*Innen auswählen.");
			return;
		}

		startTime = startTime.ToUniversalTime();
		var container = _location.GetShiftContainerByTime(startTime);
		if (container is null)
		{
			Logger.LogWarning("No container within this start time {Start}", startTime);
			return;
		}

		startTime = container.GetBestShiftStartTimeForTime(startTime).ToUniversalTime();
		var shiftResponse = new GetShiftResponse
		{
			ContainerId = container.Id,
			EmployeeId = _userKeycloakId,
			EmployeeFullName = _user.GetFullName(),
			Start = startTime,
			End = startTime + container.Framework.TimePerShift,
			Type = type?.MapToShiftTypeResponse()
		};
		var param = new Dictionary<string, object>
		{
			[nameof(EditShiftDialog.EntityToEdit)] = shiftResponse
		};
		var res = await DialogService.OpenAsync<EditShiftDialog>("Neue Schicht", param);
		if (res is true)
		{
			try
			{
				await ForceReloadScheduler();
			}
			catch (Exception ex)
			{
				await DialogService.Error(ex);
			}
		}
	}

	private async Task OnShiftSelect(SchedulerAppointmentSelectEventArgs<Appointment> args)
	{
		//If appoint has no date and is WeekView, go to DayView of selected appointment
		if (args.Data is WeekAppointment && SelectedViewIndex == WeekViewIndex)
		{
			SelectedViewIndex = DayViewIndex;
			StartDate = args.Start.Date;
			await SetShifts(SelectedViewIndex, true);
			return;
		}

		if (args.Data is not DayAppointment dayAppointment)
			throw new NotSupportedException("Unknown appointment type" + args.Data.GetType());

		//If the user is not assigned, create a new shift
		if (dayAppointment.Shift.User == Mappers.NotAssignedEmployee)
		{
			await OnSlotSelect(args.Start, dayAppointment.Shift?.Type);
			return;
		}

		var param = new Dictionary<string, object>
		{
			[nameof(EditShiftDialog.EntityToEdit)] = dayAppointment.Shift.MapToShiftResponse()
		};
		var res = await DialogService.OpenAsync<EditShiftDialog>("Bearbeite Schicht", param);
		if (res is true)
		{
			await ForceReloadScheduler();
		}
	}

	// Never call StateHasChanged in AppointmentRender - would lead to infinite loop
	//TODO[perf] OnAppointmentRender will be called twice, on day/week leave and on day/week come
	private void OnAppointmentRender(SchedulerAppointmentRenderEventArgs<Appointment> args)
	{
		args.SetAppointmentRenderStyle(_userKeycloakId);
	}


	private void OnSlotRender(SchedulerSlotRenderEventArgs args)
		=> args.SetSlotRenderStyle(_location!.Containers);


	private readonly SemaphoreSlim _semaphore = new(1, 1);

	private async Task SetShifts(int idx, bool invokeStateHasChanged = false)
	{
		await _semaphore.WaitAsync();
		try
		{
			_shifts.Clear();
			var start = ShiftService.CurrentSeason.StartDate;
			var end = ShiftService.CurrentSeason.EndDate;
			switch (idx)
			{
				case WeekViewIndex:
				{
					//Week view
					var myShifts =
						(await ShiftService.GetAllShiftsFromLocationAsync(Id, start, end, _userKeycloakId))
						.ToList();
					var shiftTypes =
						await ShiftService.GetAllAvailableShiftTypesFromLocationAsync(Id, start, end);
					var group = shiftTypes.GroupBy(st => new { st.Start, st.End });
					var appointments = group.Select(g => CreateNewAppointment(g.Key.Start, g.Key.End, g, myShifts));
					_shifts = appointments.ToList();
					SelectedViewIndex = 1;
					UpdateQueryUri();
					break;
				}
				case DayViewIndex:
				{
					//day view
					var shifts = (await ShiftService.GetAllShiftsFromLocationAsync(Id, start, end)).ToList();
					ShiftService.FillShiftsWithUnassignedShifts(ref shifts, _location!.Containers, start, end);
					_shifts = shifts.OrderBy(q => q.Type.Id).Select(s => (Appointment)s.ToAppointment()).ToList();
					SelectedViewIndex = 0;
					UpdateQueryUri();
					break;
				}
			}

			if (invokeStateHasChanged)
				await InvokeAsync(StateHasChanged);
		}
		finally
		{
			_semaphore.Release();
		}
	}

	private Appointment CreateNewAppointment(DateTime start, DateTime end,
		IEnumerable<GetShiftTypesCountResponse> getShiftTypesResponses, List<Shift> myShifts)
	{
		if (myShifts.FirstOrDefault(s => s.StartTime == start) is { } shift)
		{
			return shift.ToAppointment();
		}

		if (!_isAdmin)
			getShiftTypesResponses = getShiftTypesResponses.Where(s => s.Type.OnlyAssignableByAdmin == false);

		var arr = getShiftTypesResponses as GetShiftTypesCountResponse[] ?? getShiftTypesResponses.ToArray();
		var total = arr.Sum(s => s.TotalCount);
		var available = Math.Max(0, arr.Sum(s => s.AvailableCount));
		// var available = Random.Shared.Next(0, total + 1);
		var title = $"{available}/{total}\nfreie Schicht{(available != 1 ? "en" : "")}";
		return new WeekAppointment(start, end, title, available, total);
	}


	private bool _isAdmin;
	private DateTime _startDate;
	private RadzenDayView _dayView;
	private RadzenWeekView _weekView;
	[Inject] private NavigationManager NavigationManager { get; set; }

	private void UpdateQueryUri(DateTime date = default)
	{
		if (_scheduler is null)
			return;
		StartDate = (date == default ? _scheduler.CurrentDate.Date : date);
		var queryParams = new Dictionary<string, object?>
		{
			[nameof(ShowOnlyUsersShifts)] = ShowOnlyUsersShifts,
			[nameof(StartDate)] = StartDate.ToString("yyyy-MM-dd"),
			[nameof(SelectedViewIndex)] = SelectedViewIndex
		};
		_ = JsRuntime.InvokeVoidAsync("updateQueryParameters", queryParams).AsTask();
		InvokeAsync(StateHasChanged); //For the DatePicker
	}

	private void ShowOnlyUserShiftsButtonPressed()
	{
		ShowOnlyUsersShifts = !ShowOnlyUsersShifts;
		UpdateQueryUri();
	}

	private async Task Swipe(SwipeEvent obj)
	{
		Console.WriteLine("<>");
		if (_scheduler is null)
			return;
		ISchedulerView? view = GetSelectedViewIndex() switch
		{
			DayViewIndex => _dayView,
			WeekViewIndex => _weekView,
			_ => null
		};
		if (view is null)
			return;
		_scheduler.CurrentDate = obj switch
		{
			SwipeEvent.Left => view.Next(),
			SwipeEvent.Right => view.Prev(),
			_ => _scheduler.CurrentDate
		};
		await _scheduler.Reload();
	}

	private const int WeekViewIndex = 1;
	private const int DayViewIndex = 0;

	private void LoadShifts(SchedulerLoadDataEventArgs obj)
	{
		UpdateQueryUri();
		var current = GetSelectedViewIndex();
		if (SelectedViewIndex == current && _shifts.Count > 0)
			return;
		_ = SetShifts(current, true);
	}
}