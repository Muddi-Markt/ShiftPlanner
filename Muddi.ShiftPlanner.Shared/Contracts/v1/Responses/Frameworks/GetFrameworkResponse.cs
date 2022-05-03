namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Responses.Frameworks;

public class GetFrameworkResponse : IMuddiResponse
{
	public Guid Id { get; set; }
	public int TimePerShift { get; set; }
	public IEnumerable<ShiftFrameworkTypeCountDto> ShiftTypeCounts { get; set; }
}