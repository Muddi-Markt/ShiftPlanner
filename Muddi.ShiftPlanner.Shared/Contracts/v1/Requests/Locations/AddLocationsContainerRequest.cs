namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Requests;

public class AddLocationsContainerRequest
{
	public Guid LocationId { get; set; }
	public IEnumerable<Guid> ContainerIds { get; set; }
}