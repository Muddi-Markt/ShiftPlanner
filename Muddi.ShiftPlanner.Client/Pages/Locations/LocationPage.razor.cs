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
	[Parameter] public Guid Id { get; set; }

	[SupplyParameterFromQuery] [Parameter] public bool ShowOnlyUsersShifts { get; set; }
	[SupplyParameterFromQuery] [Parameter] public int SelectedViewIndex { get; set; }

	[SupplyParameterFromQuery]
	[Parameter]
	public DateTime StartDate
	{
		get => _startDate;
		set => _startDate = value == default ? _startDate : value;
	}


	[CascadingParameter] public Task<AuthenticationState> AuthenticationState { get; set; }

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
			_user = state.User;
			_userKeycloakId = _user.GetKeycloakId();
			_isAdmin = _user.IsInRole(ApiRoles.Admin);
			_scheduler?.Reload();
		}
		catch (Exception ex)
		{
			if (ex is AccessTokenNotAvailableException)
				throw;
			await DialogService.Error(ex, "Error while setting parameter of places page");
		}
	}


	private Task OnSlotSelect(SchedulerSlotSelectEventArgs args)
	{
		return OnSlotSelect(args.Start);
	}

	private async Task OnSlotSelect(DateTime startTime, ShiftType? type = null)
	{
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
			Employee = new() { Id = _user.GetKeycloakId(), UserName = _user.GetFullName() },
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
				await _scheduler.Reload();
			}
			catch (Exception ex)
			{
				await DialogService.Error(ex);
			}
		}
	}

	private async Task OnShiftSelect(SchedulerAppointmentSelectEventArgs<Appointment> args)
	{
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
			await _scheduler.Reload();
		}
	}

	// Never call StateHasChanged in AppointmentRender - would lead to infinite loop
	private void OnAppointmentRender(SchedulerAppointmentRenderEventArgs<Appointment> args)
		=> args.SetAppointmentRenderStyle(_userKeycloakId);


	private void OnSlotRender(SchedulerSlotRenderEventArgs args)
		=> args.SetSlotRenderStyle(_location!.Containers);


	private async Task LoadShifts(SchedulerLoadDataEventArgs arg)
	{
		_isLoading = true;
		_shifts.Clear();
		if (arg.End - arg.Start > TimeSpan.FromDays(1))
		{
			//Week view
			var myShifts = (await ShiftService.GetAllShiftsFromLocationAsync(Id, arg.Start, arg.End, _userKeycloakId)).ToList();
			var shiftTypes = await ShiftService.GetAllAvailableShiftTypesFromLocationAsync(Id, arg.Start, arg.End);
			var group = shiftTypes.GroupBy(st => new { st.Start, st.End });
			var appt = group.Select(g => CreateNewAppointment(g.Key.Start, g.Key.End, g, myShifts));
			_shifts = appt.ToList();
			SelectedViewIndex = 1;
			UpdateQueryUri();
		}
		else
		{
			//day view
			var shifts = (await ShiftService.GetAllShiftsFromLocationAsync(Id, arg.Start, arg.End)).ToList();
			ShiftService.FillShiftsWithUnassignedShifts(ref shifts, _location!.Containers, arg.Start, arg.End);
			_shifts = shifts.OrderBy(q => q.Type.Id).Select(s => s.ToAppointment()).ToList();
			SelectedViewIndex = 0;
			UpdateQueryUri();
		}

		_isLoading = false;
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
		var available = arr.Sum(s => s.AvailableCount);
		var total = arr.Sum(s => s.TotalCount);
		var title = $"{available}/{total}\nfreie Schicht{(available != 1 ? "en" : "")}";
		return new Appointment(start, end, title);
	}


	private bool _isAdmin;
	private DateTime _startDate = DateTime.Now > GlobalSettings.FirstDate ? DateTime.Now : GlobalSettings.FirstDate;
	private RadzenDayViewFix _dayView;
	private RadzenWeekView _weekView;
	[Inject] private NavigationManager NavigationManager { get; set; }

	private void UpdateQueryUri()
	{
		var s = NavigationManager.GetUriWithQueryParameters(new Dictionary<string, object?>
		{
			[nameof(ShowOnlyUsersShifts)] = ShowOnlyUsersShifts,
			[nameof(StartDate)] = _scheduler?.CurrentDate.Date.ToString("yyyy-MM-dd"),
			[nameof(SelectedViewIndex)] = SelectedViewIndex
		});
		NavigationManager.NavigateTo(s);
		// return Task.CompletedTask;
	}

	private async Task Swipe(SwipeEvent obj)
	{
		if (_scheduler is null)
			return;
		ISchedulerView? view = _scheduler.IsSelected(_dayView)
			? _dayView
			: _scheduler.IsSelected(_weekView)
				? _weekView
				: null;
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
}