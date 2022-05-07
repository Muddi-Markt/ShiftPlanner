namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Responses;

public class ShiftFrameworkTypeCountResponse
{
	public GetShiftTypesResponse ShiftType { get; set; }
	public int Count { get; set; }
}