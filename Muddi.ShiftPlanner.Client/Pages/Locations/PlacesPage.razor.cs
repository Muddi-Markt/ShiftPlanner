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
	private IList<Shift> _shifts = new List<Shift>();
	private Guid _userKeycloakId;

	protected override async Task OnParametersSetAsync()
	{
		try
		{
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


	private Task OnSlotSelect(SchedulerSlotSelectEventArgs args)
	{
		return OnSlotSelect(args.Start);
	}

	private async Task OnSlotSelect(DateTime startTime, ShiftType? type = null)
	{
		startTime = startTime.ToUniversalTime();
		ShiftContainer container = _location.GetShiftContainerByTime(startTime);
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
		if (args.Data.User == _notAssignedEmployee)
		{
			await OnSlotSelect(args.Start,args.Data.Type);
			return;
		}
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
		args.Attributes["style"] = string.Empty;
		if (args.Data.User == _notAssignedEmployee)
		{
			string color = args.Data.Type.Color.EndsWith(')')
				? $"{args.Data.Type.Color[..^1]}, 30%)"
				: "#00000077";
				
			args.Attributes["style"] += $"background: {args.Data.Type.Color};backdrop-filter: blur(2px);mix-blend-mode:color-burn;";
			return;
		}

		args.Attributes["style"] += $"background: {args.Data.Type.Color};";
		if (args.Data.User.KeycloakId == _userKeycloakId)
		{
			args.Attributes["style"] += "border: 2px dashed #852121;";
		}
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
		var shifts = (await ShiftService.GetAllShiftsFromLocationAsync(Id, arg.Start, arg.End)).ToList();
		FillShiftsWithUnassignedShifts(ref shifts,arg);
		_shifts = shifts.OrderBy(q => q.Type.Id).ToList();
		_isLoading = false;
	}

	private static NotAssignedEmployee _notAssignedEmployee = new();

	private void FillShiftsWithUnassignedShifts(ref List<Shift> shifts, SchedulerLoadDataEventArgs arg)
	{
		var startTime = arg.Start.ToUniversalTime();
		var endTime = arg.End.ToUniversalTime();
		foreach (var container in _location!.Containers)
		{
			foreach (var containerStart in container.ShiftStartTimes)
			{

				if (containerStart <startTime || containerStart >= endTime)
					continue;
				foreach (var (type, count) in container.Framework.RolesCount)
				{
					var assignedShiftsCount = shifts.Count(s =>
						s.ContainerId == container.Id
						&& s.Type == type
						&& s.StartTime == containerStart);
					if (assignedShiftsCount < count)
					{
						if (type.Name == "Muddi in Charge")
							Console.WriteLine(containerStart + " " + assignedShiftsCount + " / " + count);

						for (int i = 0; i < count - assignedShiftsCount; i++)
						{
							if (type.Name == "Muddi in Charge")
								Console.WriteLine(i);
							shifts.Add(new Shift(_notAssignedEmployee, containerStart, containerStart + container.Framework.TimePerShift, type));
						}
					}
				}
			}
		}
	}
}