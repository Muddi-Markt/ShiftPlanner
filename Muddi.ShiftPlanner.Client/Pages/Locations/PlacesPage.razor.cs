using System.Collections.Concurrent;
using Microsoft.AspNetCore.Components;
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

	private ShiftLocation _location;

	private readonly List<AppointmentData> _appointments = new()
	{
		new() { Start = DateTime.Now, End = DateTime.Now.AddHours(2), Text = "hallo welt" }
	};

	private RadzenScheduler<AppointmentData> _scheduler;

	protected override async Task OnParametersSetAsync()
	{
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
				var c = _colorsPerFramework.GetOrAdd(container.GetHashCode(), _ => _freeBackgroundColors.TryPop(out var color) ? color : "#f00");
				if (args.Start >= container.StartTime && args.Start < container.EndTime)
					args.Attributes["style"] = $"background: {c};";
			}
			

		}
	}

	private async Task OnSlotSelect(SchedulerSlotSelectEventArgs args)
	{
		AppointmentData data = await DialogService.OpenAsync<EditShiftComponent>("Add Appointment",
			new Dictionary<string, object> { { "Appointment", new AppointmentData() { Start = args.Start, End = args.End } } });
		Console.WriteLine($"Data is {data}");
		if (data != null)
		{
			_appointments.Add(data);
			// Either call the Reload method or reassign the Data property of the Scheduler
			await _scheduler.Reload();
			// await InvokeAsync(StateHasChanged);
		}
	}

	private async Task OnAppointmentSelect(SchedulerAppointmentSelectEventArgs<AppointmentData> args)
	{
		await DialogService.OpenAsync<EditShiftComponent>("Edit Appointment",
			new Dictionary<string, object> { { "Appointment", args.Data } });

		await _scheduler.Reload();
	}

	private static void OnAppointmentRender(SchedulerAppointmentRenderEventArgs<AppointmentData> args)
	{
		// Never call StateHasChanged in AppointmentRender - would lead to infinite loop

		if (args.Data.Text == "Birthday")
		{
			args.Attributes["style"] = "background: red";
		}
	}

	private ConcurrentDictionary<int, string> _colorsPerFramework = new();

	private ConcurrentStack<string> _freeBackgroundColors = new();
	private static string[] FrameworkBackgroundColors = {
		"rgba(255,220,40,.2)",
		"rgba(255,40,220,.2)",
		"rgba(40,255,220,.2)",
		"rgba(40,220,255,.2)"
	};
}