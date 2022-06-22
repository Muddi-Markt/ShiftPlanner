namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Requests;

public class GetAllShiftsRequest
{
	public int? Limit { get; set; }
	public DateTime? StartingFrom { get; set; }
}