using Microsoft.AspNetCore.Components;
using Muddi.ShiftPlanner.Shared.Api;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses;
using Muddi.ShiftPlanner.Shared.Entities;

namespace Muddi.ShiftPlanner.Client.Pages.Statistics;

public partial class UserStatisticsPage
{
	[Inject] protected IMuddiShiftApi ShiftApi { get; set; }

	private Dictionary<GetEmployeeResponse, List<GetShiftResponse>> _employeesShifts = new();
	private List<GetShiftResponse> _shifts = new();
	private TimeSpan _totalShiftHours = default;

	protected override async Task OnInitializedAsync()
	{
		var users = await ShiftApi.GetAllEmployees();
		_shifts = await ShiftApi.GetAllShifts();
		_totalShiftHours = CalculateTotalTime(_shifts);
		_employeesShifts = users.GroupJoin(_shifts, u => u.Id, s
				=> s.EmployeeId, (user, shifts) => new { user, shifts = shifts.ToList() })
			.OrderByDescending(t => t.shifts.Count)
			.ToDictionary(k => k.user, v => v.shifts);
	}

	private static TimeSpan CalculateTotalTime(IEnumerable<GetShiftResponse> shifts)
		=> TimeSpan.FromTicks(shifts.Sum(s => (s.End - s.Start).Ticks));
}