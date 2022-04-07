using System.Collections.Concurrent;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Muddi.ShiftPlanner.Client.Entities;
using Muddi.ShiftPlanner.Client.Services;
using Muddi.ShiftPlanner.Shared;
using Muddi.ShiftPlanner.Shared.Entities;
using Muddi.ShiftPlanner.Shared.Exceptions;
using Radzen;
using Radzen.Blazor;

namespace Muddi.ShiftPlanner.Client.Pages.Locations;

public partial class PlacesPage
{
	[Inject] private DialogService DialogService { get; set; } = default!;
	[Parameter] public Guid Id { get; set; }
	[CascadingParameter] public Task<AuthenticationState> AuthenticationState { get; set; }

	private ShiftLocation _location;
	private MuddiConnectUser _user;

	private IEnumerable<Shift> Appointments => _location.GetAllShifts();

	private RadzenScheduler<Shift> _scheduler;

	protected override async Task OnParametersSetAsync()
	{
		var state = await AuthenticationState;
		_location = await ShiftMockService.GetPlaceByIdAsync(Id);
		_user = MuddiConnectUser.CreateFromClaimsPrincipal(state.User);
		_frameworkBackgroundColors.Reset();
		_shiftRoleBackgroundColors.Reset();
	}


	private async Task OnSlotSelect(SchedulerSlotSelectEventArgs args)
	{
		var container = _location.GetShiftContainerByTime(args.Start);
		DateTime startTime = container.GetBestShiftStartTimeForTime(args.Start);
		var parameter = new TemplateFormShiftParameter(container, _user, startTime);
		TemplateFormShiftParameter data = await DialogService.OpenAsync<EditShiftComponent>("Füge Schicht hinzu",
			new Dictionary<string, object> { { nameof(EditShiftComponent.ShiftParameter), parameter } });
		if (data?.Role is not null)
		{
			try
			{
				_location.AddShift(_user, data.StartTime, data.Role);
			}
			catch (MuddiException ex)
			{
				await DialogService.Confirm(ex.Message, "Ein Fehler ist aufgetreten");
			}

			// Either call the Reload method or reassign the Data property of the Scheduler
			await _scheduler.Reload();
			// await InvokeAsync(StateHasChanged);
		}
	}

	private async Task OnShiftSelect(SchedulerAppointmentSelectEventArgs<Shift> args)
	{
		var container = _location.GetShiftContainerByTime(args.Start);
		var parameter = new TemplateFormShiftParameter(container, _user, args.Data);
		TemplateFormShiftParameter data = await DialogService.OpenAsync<EditShiftComponent>("Bearbeite Schicht",
			new Dictionary<string, object> { { nameof(EditShiftComponent.ShiftParameter), parameter } });

		if (data?.Role is not null && data.ShiftToEdit is not null)
		{
			_location.UpdateShift(data.ShiftToEdit, data.Role, data.User);
		}


		await _scheduler.Reload();
	}

	private void OnAppointmentRender(SchedulerAppointmentRenderEventArgs<Shift> args)
	{
		// Never call StateHasChanged in AppointmentRender - would lead to infinite loop

		args.Attributes["style"] = $"background: {_shiftRoleBackgroundColors.GetColor(args.Data.Role)}";
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
					var c = _frameworkBackgroundColors.GetColor(container);
					if (args.Start >= container.StartTime && args.Start < container.EndTime)
						args.Attributes["style"] = $"background: {c};";
				}

				break;
			}
		}
	}

	private readonly ColorSelector<ShiftContainer> _frameworkBackgroundColors = new(new[]
	{
		"rgba(255,220,40,.4)",
		"rgba(0,0,255,.4)",
		"rgba(0,255,0,.4)",
		"rgba(255,0,0,.4)"
	});

	private readonly ColorSelector<ShiftRole> _shiftRoleBackgroundColors = new(new[]
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