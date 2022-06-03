namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Responses;

public class GetShiftsCountResponse
{
	public Guid Id { get; set; }
	public int TotalShifts { get; set; }
	public int AssignedShifts { get; set; }
}