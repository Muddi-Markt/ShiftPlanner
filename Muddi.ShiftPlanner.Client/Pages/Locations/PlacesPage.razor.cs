using System.Collections.Concurrent;
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
	private MuddiConnectUser _user;

	private List<Shift> Shifts { get; set; }
	private DateTime StartDate { get; } = (DateTime.Now > GlobalSettings.FirstDate ? DateTime.Now : GlobalSettings.FirstDate);

	private RadzenScheduler<Shift> _scheduler;

	protected override async Task OnParametersSetAsync()
	{
		try
		{
			var state = await AuthenticationState;
			_location = await ShiftService.GetLocationsByIdAsync(Id);
			_user = MuddiConnectUser.CreateFromClaimsPrincipal(state.User);
			_frameworkBackgroundColors.Reset();
			_shiftRoleBackgroundColors.Reset();
			Shifts = (await ShiftService.GetAllShiftsFromLocationAsync(Id)).ToList();
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
		var parameter = new TemplateFormShiftParameter(container, _user, startTime);
		TemplateFormShiftParameter data = await DialogService.OpenAsync<EditShiftComponent>("Füge Schicht hinzu",
			new Dictionary<string, object> { { nameof(EditShiftComponent.ShiftParameter), parameter } });
		if (data?.Role is not null)
		{
			try
			{
				Shifts.Add(new Shift(_user, data.StartTime, data.StartTime + container.Framework.TimePerShift, data.Role).ToLocalTime());
				await _scheduler.Reload();
				await InvokeAsync(StateHasChanged);
				await ShiftService.AddShiftToLocation(_location,
					new CreateLocationsShiftRequest
					{
						EmployeeKeycloakId = _user.KeycloakId,
						ShiftTypeId = data.Role.Id,
						Start = data.StartTime
					});
			}
			catch (MuddiException ex)
			{
				await DialogService.Confirm(ex.Message, "Ein Fehler ist aufgetreten");
			}

			// Either call the Reload method or reassign the Data property of the Scheduler

			// await InvokeAsync(StateHasChanged);
		}
	}

	private async Task OnShiftSelect(SchedulerAppointmentSelectEventArgs<Shift> args)
	{

		var param = new Dictionary<string, object>
		{
			[nameof(EditShiftDialog.EntityToEdit)] = args.Data.MapToShiftResponse()
		}; 
		var res = await DialogService.OpenAsync<EditShiftDialog>("Edit shift",param);
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

		args.Attributes["style"] = $"background: {_shiftRoleBackgroundColors.GetColor(args.Data.Type)}";
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
					var c = _frameworkBackgroundColors.GetColor(container.Framework.Id);
					if (args.Start >= container.StartTime.ToLocalTime() && args.Start < container.EndTime.ToLocalTime())
						args.Attributes["style"] = $"background: {c};";
				}

				break;
			}
		}
	}

	private readonly ColorSelector<Guid> _frameworkBackgroundColors = new(new[]
	{
		"rgba(255,220,40,.4)",
		"rgba(0,0,255,.4)",
		"rgba(0,255,0,.4)",
		"rgba(255,0,0,.4)"
	});

	private readonly ColorSelector<ShiftType> _shiftRoleBackgroundColors = new(new[]
	{
		"rgba(255,40,220)",
		"rgba(0,0,200)",
		"rgba(0,200,0)",
		"rgba(200,0,0)"
	});
}

public class ColorSelector<TItem> where TItem : notnull
{
	private ConcurrentDictionary<TItem, string> _colorsForItem = new();
	private readonly ConcurrentStack<string> _freeColors = new();
	private readonly string[] _colors;

	public ColorSelector(string[] colors)
	{
		_colors = colors;
		Reset();
	}

	public string GetColor(TItem item)
	{
		return _colorsForItem.GetOrAdd(item, _ => _freeColors.TryPop(out var c) ? c : "#f00");
	}

	public void Reset()
	{
		_freeColors.Clear();
		_freeColors.PushRange(_colors);
	}
}