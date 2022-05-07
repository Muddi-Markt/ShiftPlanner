namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Responses;

public class GetFrameworkResponse : IMuddiResponse
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public int SecondsPerShift { get; set; }
	public IEnumerable<ShiftFrameworkTypeCountResponse> ShiftTypeCounts { get; set; }
}