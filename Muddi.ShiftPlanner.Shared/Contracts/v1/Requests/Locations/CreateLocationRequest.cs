namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Requests.Locations;

public class CreateLocationRequest
{
	public string Name { get; set; }
	public Guid TypeId { get; set; }
}