using Microsoft.AspNetCore.Components;
using Muddi.ShiftPlanner.Client.Services;
using Muddi.ShiftPlanner.Client.Shared;
using Muddi.ShiftPlanner.Shared.Api;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses;
using Muddi.ShiftPlanner.Shared.Entities;
using Radzen;

namespace Muddi.ShiftPlanner.Client.Pages.Statistics;

public partial class UserStatisticsPage
{
	[Inject] protected IMuddiShiftApi ShiftApi { get; set; }
	[Inject] protected ShiftService ShiftService { get; set; }
	[Inject] protected DialogService DialogService { get; set; }
	[CascadingParameter] private MainLayout MainLayout { get; set; }

	private Dictionary<GetEmployeeResponse, List<GetShiftResponse>> _employeesShifts = new();
	private List<GetShiftResponse> _shifts = new();
	private TimeSpan _totalShiftHours = default;

	protected override async Task OnInitializedAsync()
	{
		var users = await ShiftApi.GetAllEmployees();
		_shifts = await ShiftApi.GetAllShifts(new() { SeasonId = ShiftService.CurrentSeason.Id });
		_totalShiftHours = CalculateTotalTime(_shifts);
		_employeesShifts = users.GroupJoin(_shifts, u => u.Id, s
				=> s.EmployeeId, (user, shifts) => new { user, shifts = shifts.ToList() })
			.OrderByDescending(t => t.shifts.Count)
			.ToDictionary(k => k.user, v => v.shifts);
	}

	private Task ShowUserShifts(GetEmployeeResponse getEmployeeResponse, IEnumerable<GetShiftResponse> shifts)
	{
		return DialogService.OpenAsync<UserShiftsDialog>($"Schichten von {getEmployeeResponse.FirstName}",
			new Dictionary<string, object>
			{
				[nameof(UserShiftsDialog.UserShifts)] = shifts
			});
	}

	private static TimeSpan CalculateTotalTime(IEnumerable<GetShiftResponse> shifts)
		=> TimeSpan.FromTicks(shifts.Sum(s => (s.End - s.Start).Ticks));
}