using System.Collections.Concurrent;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Muddi.ShiftPlanner.Client.Entities;
using Muddi.ShiftPlanner.Client.Services;
using Muddi.ShiftPlanner.Shared;
using Muddi.ShiftPlanner.Shared.Entities;
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
		_user = MuddiConnectUser.CreateFromClaimsPrincipal(state.User);
		_freeBackgroundColors.Clear();
		_freeBackgroundColors.PushRange(FrameworkBackgroundColors);
		_location = await ShiftMockService.GetPlaceByIdAsync(Id);
	}

	private void OnSlotRender(SchedulerSlotRenderEventArgs args)
	{
		// Highlight today in month view
		if (args.View.Text == "Month" && args.Start.Date == DateTime.Today)
		{
			args.Attributes["style"] = "background: rgba(255,220,40,.2);";
		}

		// Highlight shift hours
		if (args.View.Text is "Week" or "Day")
		{
			foreach (var container in _location.Containers)
			{
				var c = _colorsPerFramework.GetOrAdd(container.GetHashCode(),
					_ => _freeBackgroundColors.TryPop(out var color) ? color : "#f00");
				if (args.Start >= container.StartTime && args.Start < container.EndTime)
					args.Attributes["style"] = $"background: {c};";
			}
		}
	}

	private async Task OnSlotSelect(SchedulerSlotSelectEventArgs args)
	{
		var container = _location.GetShiftContainerByTime(args.Start);
		DateTime startTime = container.GetBestShiftStartTimeForTime(args.Start);
		var parameter = new TemplateFormShiftParameter(container, _user, startTime);
		TemplateFormShiftParameter data = await DialogService.OpenAsync<EditShiftComponent>("Add Appointment",
			new Dictionary<string, object> { { nameof(EditShiftComponent.ShiftParameter), parameter } });
		if (data?.Role is not null)
		{
			_location.AddShift(_user, data.StartTime, data.Role);
			// Either call the Reload method or reassign the Data property of the Scheduler
			await _scheduler.Reload();
			// await InvokeAsync(StateHasChanged);
		}
	}

	private async Task OnAppointmentSelect(SchedulerAppointmentSelectEventArgs<Shift> args)
	{
		var container = _location.GetShiftContainerByTime(args.Start);
		var parameter = new TemplateFormShiftParameter(container, _user, args.Data);
		await DialogService.OpenAsync<EditShiftComponent>("Edit Appointment",
			new Dictionary<string, object> { { nameof(EditShiftComponent.ShiftParameter), parameter } });

		await _scheduler.Reload();
	}

	private static void OnAppointmentRender(SchedulerAppointmentRenderEventArgs<Shift> args)
	{
		// Never call StateHasChanged in AppointmentRender - would lead to infinite loop
	}

	private ConcurrentDictionary<int, string> _colorsPerFramework = new();

	private ConcurrentStack<string> _freeBackgroundColors = new();

	private static string[] FrameworkBackgroundColors =
	{
		"rgba(255,220,40,.2)",
		"rgba(40,40,220,.2)",
		"rgba(40,255,220,.2)",
		"rgba(40,220,255,.2)"
	};
}