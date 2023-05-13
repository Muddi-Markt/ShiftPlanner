using System.Collections.Concurrent;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Muddi.ShiftPlanner.Client.Components;
using Muddi.ShiftPlanner.Client.Entities;
using Muddi.ShiftPlanner.Client.Services;
using Muddi.ShiftPlanner.Client.Shared;
using Muddi.ShiftPlanner.Shared;
using Muddi.ShiftPlanner.Shared.Contracts.v1;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Requests;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses;
using Muddi.ShiftPlanner.Shared.Entities;
using Muddi.ShiftPlanner.Shared.Exceptions;
using Radzen;
using Radzen.Blazor;

namespace Muddi.ShiftPlanner.Client.Pages.Locations;

public partial class LocationPage
{
	[Inject] private DialogService DialogService { get; set; } = default!;
	[Inject] private ShiftService ShiftService { get; set; } = default!;
	[Inject] private ILogger<LocationPage> Logger { get; set; } = default!;

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

	private bool _isLoading = true;
	private bool _enableLoadingSpinner = true;

	private IEnumerable<Appointment> Shifts => ShowOnlyUsersShifts
		? _shifts.Where(s => s.Shift?.User.KeycloakId == _userKeycloakId)
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
			var available =
				(await ShiftService.GetAllAvailableShiftTypesFromLocationAsync(Id, ShiftService.CurrentSeason.StartDate,
					ShiftService.CurrentSeason.EndDate, 1)).ToList();
			MainLayout.SetTitle($"{_location.Name} ({_location.AssignedShifts}/{_location.TotalShifts} Schichten)");
			_user = state.User;
			if (_startDate == default)
			{
				var startDate =
					(available.FirstOrDefault()?.Start ?? ShiftService.CurrentSeason.StartDate).ToLocalTime();
				Console.WriteLine("startDate: " + available.FirstOrDefault()?.Start.Kind);
				Console.WriteLine("CurrentSeason: " + ShiftService.CurrentSeason.StartDate.Kind);
				Console.WriteLine("StartDate: " + StartDate.Kind);
				_startDate = DateTime.Now > startDate ? DateTime.Now : startDate;
			}

			_userKeycloakId = _user.GetKeycloakId();
			_isAdmin = _user.IsInRole(ApiRoles.Admin);
			await ForceReloadScheduler();
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
		_oldStart = default;
		_oldEnd = default;
		return _scheduler?.Reload() ?? Task.CompletedTask;
	}


	private Task OnSlotSelect(SchedulerSlotSelectEventArgs args)
	{
		return OnSlotSelect(args.Start);
	}

	private async Task OnSlotSelect(DateTime startTime, ShiftType? type = null)
	{
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
		if (args.Data.Shift is null && SelectedViewIndex == WeekViewIndex)
		{
			SelectedViewIndex = DayViewIndex;
			StartDate = args.Start.Date;
			return;
		}

		//If shift is null or the user is not assigned, create a new shift
		if (args.Data.Shift is null || args.Data.Shift.User == Mappers.NotAssignedEmployee)
		{
			await OnSlotSelect(args.Start, args.Data.Shift?.Type);
			return;
		}

		var param = new Dictionary<string, object>
		{
			[nameof(EditShiftDialog.EntityToEdit)] = args.Data.Shift.MapToShiftResponse()
		};
		var res = await DialogService.OpenAsync<EditShiftDialog>("Bearbeite Schicht", param);
		if (res is true)
		{
			await ForceReloadScheduler();
		}
	}

	// Never call StateHasChanged in AppointmentRender - would lead to infinite loop
	private void OnAppointmentRender(SchedulerAppointmentRenderEventArgs<Appointment> args)
		=> args.SetAppointmentRenderStyle(_userKeycloakId);


	private void OnSlotRender(SchedulerSlotRenderEventArgs args)
		=> args.SetSlotRenderStyle(_location!.Containers);


	private DateTime _oldStart;
	private DateTime _oldEnd;


	private SemaphoreSlim _semaphore = new(1, 1);

	private async Task LoadShifts(SchedulerLoadDataEventArgs arg)
	{
		await _semaphore.WaitAsync();
		try
		{
			var idx = GetSelectedViewIndex();
			bool shiftsAny = _shifts.Any();
			bool dateNotChanged = arg.Start == _oldStart && arg.End == _oldEnd;
			//I dont know why, but we need this
			bool isCorrectFormat = (idx == WeekViewIndex && arg.End - arg.Start > TimeSpan.FromDays(1))
			                       || (idx == DayViewIndex && arg.End - arg.Start <= TimeSpan.FromDays(1));
			if ((shiftsAny && dateNotChanged) || !isCorrectFormat)
			{
				return;
			}

			Console.WriteLine(arg.Start);
			Console.WriteLine(arg.Start.Kind);
			_oldStart = arg.Start;
			_oldEnd = arg.End;
			_isLoading = true;
			_shifts.Clear();

			switch (idx)
			{
				case WeekViewIndex:
				{
					//Week view
					var myShifts =
						(await ShiftService.GetAllShiftsFromLocationAsync(Id, arg.Start, arg.End, _userKeycloakId))
						.ToList();
					var shiftTypes =
						await ShiftService.GetAllAvailableShiftTypesFromLocationAsync(Id, arg.Start, arg.End);
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
					var shifts = (await ShiftService.GetAllShiftsFromLocationAsync(Id, arg.Start, arg.End)).ToList();
					ShiftService.FillShiftsWithUnassignedShifts(ref shifts, _location!.Containers, arg.Start, arg.End);
					_shifts = shifts.OrderBy(q => q.Type.Id).Select(s => s.ToAppointment()).ToList();
					SelectedViewIndex = 0;
					UpdateQueryUri();
					break;
				}
			}
		}
		finally
		{
			_isLoading = false;
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
		var available = Math.Max(0, arr.Sum(s => s.AvailableCount));
		var total = arr.Sum(s => s.TotalCount);
		var title = $"{available}/{total}\nfreie Schicht{(available != 1 ? "en" : "")}";
		return new Appointment(start, end, title);
	}


	private bool _isAdmin;
	private DateTime _startDate;
	private RadzenDayViewFix _dayView;
	private RadzenWeekView _weekView;
	[Inject] private NavigationManager NavigationManager { get; set; }

	private void UpdateQueryUri(DateTime date = default)
	{
		var sDate = (date == default ? _scheduler?.CurrentDate.Date : date)?.ToString("yyyy-MM-dd");
		var s = NavigationManager.GetUriWithQueryParameters(new Dictionary<string, object?>
		{
			[nameof(ShowOnlyUsersShifts)] = ShowOnlyUsersShifts,
			[nameof(StartDate)] = sDate,
			[nameof(SelectedViewIndex)] = SelectedViewIndex
		});
		NavigationManager.NavigateTo(s);
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
		_enableLoadingSpinner = view == _weekView;
		await _scheduler.Reload();
		_enableLoadingSpinner = true;
	}

	private const int WeekViewIndex = 1;
	private const int DayViewIndex = 0;
}