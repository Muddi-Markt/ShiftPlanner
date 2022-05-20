using System.Collections.Concurrent;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Muddi.ShiftPlanner.Client.Entities;
using Muddi.ShiftPlanner.Client.Services;
using Muddi.ShiftPlanner.Shared;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Requests;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses;
using Muddi.ShiftPlanner.Shared.Entities;
using Muddi.ShiftPlanner.Shared.Exceptions;
using Radzen;
using Radzen.Blazor;

namespace Muddi.ShiftPlanner.Client.Pages.Locations;

public partial class PlacesPage
{
	[Inject] private DialogService DialogService { get; set; } = default!;
	[Inject] private ShiftService ShiftService { get; set; } = default!;
	[Parameter] public Guid Id { get; set; }
	[CascadingParameter] public Task<AuthenticationState> AuthenticationState { get; set; }

	private ShiftLocation? _location;
	private ClaimsPrincipal? _user;

	private bool _isLoading = true;

	private IEnumerable<Shift> Shifts => _showOnlyUsersShifts 
		? _shifts.Where(s => s.User.KeycloakId == _userKeycloakId) 
		: _shifts;
	private DateTime StartDate { get; } = (DateTime.Now > GlobalSettings.FirstDate ? DateTime.Now : GlobalSettings.FirstDate);

	private RadzenScheduler<Shift> _scheduler;
	private bool _showOnlyUsersShifts;
	private IEnumerable<Shift> _shifts = Enumerable.Empty<Shift>();
	private Guid _userKeycloakId;

	protected override async Task OnParametersSetAsync()
	{
		try
		{
			var sw = Stopwatch.StartNew();
			var state = await AuthenticationState;
			_location = await ShiftService.GetLocationsByIdAsync(Id);
			_user = state.User;
			_userKeycloakId = _user.GetKeycloakId();
		}
		catch (Exception ex)
		{
			await DialogService.Error(ex, "Error while setting parameter of places page");
		}
	}


	private async Task OnSlotSelect(SchedulerSlotSelectEventArgs args)
	{
		var start = args.Start.ToUniversalTime();
		ShiftContainer container = _location.GetShiftContainerByTime(start);
		DateTime startTime = container.GetBestShiftStartTimeForTime(start).ToUniversalTime();
		var shiftResponse = new GetShiftResponse
		{
			ContainerId = container.Id,
			Employee = new() { Id = _user.GetKeycloakId(), UserName = _user.GetFullName() },
			Start = startTime
		};
		var param = new Dictionary<string, object>
		{
			[nameof(EditShiftDialog.EntityToEdit)] = shiftResponse
		};
		var res = await DialogService.OpenAsync<EditShiftDialog>("Edit shift", param);
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

	private async Task OnShiftSelect(SchedulerAppointmentSelectEventArgs<Shift> args)
	{
		var param = new Dictionary<string, object>
		{
			[nameof(EditShiftDialog.EntityToEdit)] = args.Data.MapToShiftResponse()
		};
		var res = await DialogService.OpenAsync<EditShiftDialog>("Edit shift", param);
		if (res is true)
		{
			await _scheduler.Reload();
		}
	}

	private void OnAppointmentRender(SchedulerAppointmentRenderEventArgs<Shift> args)
	{
		// Never call StateHasChanged in AppointmentRender - would lead to infinite loop
		args.Attributes["style"] = $"background: {args.Data.Type.Color}";
	}

	private void OnSlotRender(SchedulerSlotRenderEventArgs args)
	{
		switch (args.View.Text)
		{
			// Highlight today in month view
			case "Monat" when args.Start.Date == DateTime.Today:
				args.Attributes["style"] = "background: rgba(255,220,40,.2);";
				break;
			// Highlight shift hours
			case "Woche" or "Tag":
			{
				foreach (var container in _location.Containers)
				{
					var c = container.BackgroundColor;
					if (args.Start >= container.StartTime.ToLocalTime() && args.Start < container.EndTime.ToLocalTime())
						args.Attributes["style"] = $"background: {c};";
				}

				break;
			}
		}
	}

	private async Task LoadShifts(SchedulerLoadDataEventArgs arg)
	{
		_isLoading = true;
		_shifts = (await ShiftService.GetAllShiftsFromLocationAsync(Id, arg.Start, arg.End)).OrderBy(q => q.Type.Id);
		var temp = _shifts.ToList();
		await Task.Delay(500);
		_isLoading = false;
	}
}