namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Requests;

public class GetAvailableShiftsForLocationRequest
{
	public Guid LocationId { get; set; }
	public DateTime? StartTime { get; set; }
	public DateTime? EndTime { get; set; }
}