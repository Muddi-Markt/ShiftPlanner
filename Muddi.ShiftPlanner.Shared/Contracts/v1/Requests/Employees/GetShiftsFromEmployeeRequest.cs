namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Requests;

public class GetShiftsFromEmployeeRequest
{
	/// <summary>
	/// EmployeeID
	/// </summary>
	public Guid? Id { get; set; }

	/// <summary>
	/// Most recent count, if smaller then 0 all will be shown
	/// </summary>
	public int? Count { get; set; } = -1;
}