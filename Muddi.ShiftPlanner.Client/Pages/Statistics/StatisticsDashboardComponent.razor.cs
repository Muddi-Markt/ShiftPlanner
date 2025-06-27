using Microsoft.AspNetCore.Components;
using Muddi.ShiftPlanner.Client.Services;
using Muddi.ShiftPlanner.Shared.Api;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Requests;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses;
using Radzen;

namespace Muddi.ShiftPlanner.Client.Pages.Statistics;

public partial class StatisticsDashboardComponent : ComponentBase, IDisposable
{
	private Timer? _timer;
	private string TotalUsers { get; set; } = "-";
	private string TotalShifts { get; set; } = "-";
	private string TotalDays { get; set; } = "-";
	private string TotalPercentage { get; set; } = "-";
	private int TotalShiftsCount { get; set; }
	private int AvailableCount { get; set; }
	private TimeSpan TotalTimeSpan { get; set; }
	[Inject] private TooltipService TooltipService { get; set; }

	// Inject your data service here
	[Inject] protected IMuddiShiftApi ShiftApi { get; set; }
	[Inject] protected ShiftService ShiftService { get; set; }

	protected override void OnInitialized()
	{
		ShiftService.OnSeasonChanged += RefreshData;
	}

	protected override void OnParametersSet()
	{
		_ = RefreshData();
	}

	private void ShowTooltip(ElementReference elementReference, string text, TooltipOptions? options = null)
		=> TooltipService.Open(elementReference, text, options ?? new TooltipOptions
		{
			Position = TooltipPosition.Bottom, Style = "text-wrap: wrap; max-width:150px" , Duration = 60000
		});

	private void Randomize()
	{
		TotalUsers = Random.Shared.Next(100, 500).ToString();
		TotalShifts = Random.Shared.Next(1000, 5000).ToString();
		TotalDays = Random.Shared.Next(10, 99).ToString("N1");
		TotalPercentage = Random.Shared.Next(10, 99).ToString();
		InvokeAsync(StateHasChanged);
	}

	private async Task LoadDashboardData()
	{
		try
		{
			var request = new GetShiftTypesCountRequest
			{
				SeasonId = ShiftService.CurrentSeason.Id,
				IncludeNonAvailable = true
			};
			var users = await ShiftApi.GetAllEmployees();
			var allAvailableShifts = await ShiftApi.GetAvailableShiftTypes(request);
			
			var totalShiftHours = CalculateTotalTime(allAvailableShifts);
			
			if (_timer is not null)
				await _timer.DisposeAsync();
			_timer = null;

			AvailableCount = 0;
			TotalShiftsCount = 0;
			foreach (var resp in allAvailableShifts)
			{
				AvailableCount += resp.AvailableCount;
				TotalShiftsCount += resp.TotalCount;
			}


			TotalUsers = users.Count().ToString();
			TotalShifts = (TotalShiftsCount - AvailableCount).ToString();
			TotalDays = totalShiftHours.TotalDays.ToString("N1");
			TotalTimeSpan = totalShiftHours;
			if (TotalShiftsCount > 0)
				TotalPercentage = (100 * (TotalShiftsCount - AvailableCount) / TotalShiftsCount).ToString("N0");
		}
		catch (Exception ex)
		{
			// Handle error appropriately
			Console.WriteLine($"Error loading dashboard data: {ex.Message}");
		}

		await InvokeAsync(StateHasChanged);
	}


	private static TimeSpan CalculateTotalTime(ICollection<GetShiftTypesCountResponse> shifts)
	{
		var timespan = TimeSpan.Zero;
		foreach (var shift in shifts)
		{
			timespan += (shift.End - shift.Start) * (shift.TotalCount - shift.AvailableCount);
		}

		return timespan;
	}


	private async Task RefreshData()
	{
		_timer?.Dispose();
		_timer = new Timer(_ => Randomize(), null, TimeSpan.FromMilliseconds(300), TimeSpan.FromMilliseconds(50));
		await LoadDashboardData();
	}

	public void Dispose()
	{
		_timer?.Dispose();
		ShiftService.OnSeasonChanged -= RefreshData;
	}
}