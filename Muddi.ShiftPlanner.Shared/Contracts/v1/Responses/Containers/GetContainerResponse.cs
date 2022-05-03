using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses.Frameworks;

namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Responses.Containers;

public class GetContainerResponse : IMuddiResponse
{
	public Guid Id { get; set; }
	public DateTime	Start { get; set; }
	public int TotalShifts { get; set; }
	public GetFrameworkResponse ShiftFramework { get; set; }
}