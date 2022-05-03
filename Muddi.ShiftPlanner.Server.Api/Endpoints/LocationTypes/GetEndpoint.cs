using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses.LocationTypes;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.LocationTypes;

public class GetEndpoint : CrudGetEndpoint<GetLocationTypesResponse>
{


	protected override void CrudConfigure()
	{
		Get("/location-types/{Id}");
	}

	public GetEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	public override async Task<GetLocationTypesResponse?> MuddiExecuteAsync(Guid id, CancellationToken ct)
	{
		var types = await Database.ShiftLocationTypes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken: ct);
		return types?.Adapt<GetLocationTypesResponse>();
	}
}