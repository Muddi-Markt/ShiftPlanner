using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses.Containers;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses.Frameworks;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses.LocationTypes;

namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Responses.Locations;

public class GetLocationResponse : IMuddiResponse
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public GetLocationTypesResponse Type { get; set; }
	public IEnumerable<GetContainerResponse> Containers { get; set; }
}