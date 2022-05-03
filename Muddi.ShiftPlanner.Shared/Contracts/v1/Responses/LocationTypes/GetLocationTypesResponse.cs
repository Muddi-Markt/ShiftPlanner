using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses.Frameworks;

namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Responses.LocationTypes;

public class GetLocationTypesResponse : IMuddiResponse
{
	public Guid Id { get; set; }
	public string Name { get; set; }
}