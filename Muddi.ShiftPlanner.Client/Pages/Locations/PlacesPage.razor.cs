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

	private ShiftLocation _location;
	private ClaimsPrincipal _user;

	private List<Shift> Shifts { get; set; }
	private DateTime StartDate { get; } = (DateTime.Now > GlobalSettings.FirstDate ? DateTime.Now : GlobalSettings.FirstDate);

	private RadzenScheduler<Shift> _scheduler;

	protected override async Task OnParametersSetAsync()
	{
		try
		{
			var sw = Stopwatch.StartNew();
			Console.WriteLine($"user... @ {sw.ElapsedMilliseconds}");
			var state = await AuthenticationState;
			Console.WriteLine($"locationById... @ {sw.ElapsedMilliseconds}");
			Console.WriteLine("locationById...");
			_location = await ShiftService.GetLocationsByIdAsync(Id);
			_user = state.User;
			Console.WriteLine($"done... @ {sw.ElapsedMilliseconds}");
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
				var shift = await ShiftService.GetShiftById(shiftResponse.Id);
				if (shift is not null)
				{
					Shifts.Add(shift);
					await _scheduler.Reload();
				}
			}
			catch (Exception ex)
			{
				await DialogService.Error(ex);
			}
		}


		return;
		// ShiftContainer container = _location.GetShiftContainerByTime(start);
		// DateTime startTime = container.GetBestShiftStartTimeForTime(start).ToUniversalTime();
		// var parameter = new TemplateFormShiftParameter(container, _user, startTime);
		// TemplateFormShiftParameter data = await DialogService.OpenAsync<EditShiftComponent>("Füge Schicht hinzu",
		// 	new Dictionary<string, object> { { nameof(EditShiftComponent.ShiftParameter), parameter } });
		// if (data?.Role is not null)
		// {
		// 	try
		// 	{
		// 		Shifts.Add(new Shift(_user, data.StartTime, data.StartTime + container.Framework.TimePerShift, data.Role).ToLocalTime());
		// 		await _scheduler.Reload();
		// 		await InvokeAsync(StateHasChanged);
		// 		await ShiftService.AddShiftToLocation(_location,
		// 			new CreateLocationsShiftRequest
		// 			{
		// 				EmployeeKeycloakId = _user.KeycloakId,
		// 				ShiftTypeId = data.Role.Id,
		// 				Start = data.StartTime
		// 			});
		// 	}
		// 	catch (MuddiException ex)
		// 	{
		// 		await DialogService.Confirm(ex.Message, "Ein Fehler ist aufgetreten");
		// 	}
		//
		// 	// Either call the Reload method or reassign the Data property of the Scheduler
		//
		// 	// await InvokeAsync(StateHasChanged);
		// }
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
			Shifts.Remove(args.Data);
			var shift = await ShiftService.GetShiftById(args.Data.Id);
			if (shift is not null)
			{
				Shifts.Add(shift);
			}
		}
		//
		// var container = _location.Containers.First(c => c.Id == args.Data.ContainerId);
		// var parameter = new TemplateFormShiftParameter(container,_user, args.Data);
		// TemplateFormShiftParameter data = await DialogService.OpenAsync<EditShiftComponent>("Bearbeite Schicht",
		// 	new Dictionary<string, object> { { nameof(EditShiftComponent.ShiftParameter), parameter } });
		//
		// if (data?.Role is not null && data.ShiftToEdit is not null)
		// {
		// 	_location.UpdateShift(data.ShiftToEdit, data.Role, data.User);
		// }


		await _scheduler.Reload();
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
		Shifts = (await ShiftService.GetAllShiftsFromLocationAsync(Id, arg.Start, arg.End)).ToList();
	}
}