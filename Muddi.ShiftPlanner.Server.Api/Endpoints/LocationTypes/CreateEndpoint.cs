using Mapster;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Server.Database.Entities;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.LocationTypes;

public class CreateEndpoint : CrudCreateEndpoint<CreateLocationTypeRequest, GetLocationTypesResponse, GetEndpoint>
{
	public CreateEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Post("/location-types");
	}

	public override async Task<GetLocationTypesResponse?> CrudExecuteAsync(CreateLocationTypeRequest req, CancellationToken ct)
	{
		var type = new ShiftLocationType
		{
			Id = Guid.NewGuid(),
			Name = req.Name
		};
		Database.Add(type);
		await Database.SaveChangesAsync(ct);
		return type.Adapt<GetLocationTypesResponse>();
	}
}