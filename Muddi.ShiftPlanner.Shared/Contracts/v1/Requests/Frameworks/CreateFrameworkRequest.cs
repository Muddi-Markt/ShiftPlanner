namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Requests;

public class CreateFrameworkRequest
{
	public int SecondsPerShift { get; set; }
	public IList<ShiftFrameworkTypeCountDto> TypeCounts { get; set; }
}