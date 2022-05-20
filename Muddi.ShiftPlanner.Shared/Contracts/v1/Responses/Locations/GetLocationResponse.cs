namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Responses;

public class GetLocationResponse : IMuddiResponse
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public GetLocationTypesResponse Type { get; set; } = new();
	public IEnumerable<GetContainerResponse>? Containers { get; set; }
}