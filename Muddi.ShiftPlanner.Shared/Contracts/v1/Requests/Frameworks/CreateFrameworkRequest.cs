using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses.Frameworks;

namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Requests.Frameworks;

public class CreateFrameworkRequest
{
	public int TimePerShift { get; set; }
	public IList<ShiftFrameworkTypeCountDto> TypeCounts { get; set; }
}