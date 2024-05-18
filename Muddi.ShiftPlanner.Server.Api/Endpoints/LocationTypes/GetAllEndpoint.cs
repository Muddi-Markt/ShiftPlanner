using FastEndpoints;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.LocationTypes;

public class GetAllEndpoint : CrudGetAllEndpointWithoutRequest<GetLocationTypesResponse>
{
	protected override void CrudConfigure()
	{
		Get("/location-types");
	}

	public override async Task<List<GetLocationTypesResponse>?> CrudExecuteAsync(EmptyRequest _, CancellationToken ct)
	{
		var res = await Database.ShiftLocationTypes
			.OrderBy(slt => slt.Name)
			.Select(t => t.Adapt<GetLocationTypesResponse>()).ToListAsync(ct);
		return res;
	}

	public GetAllEndpoint(ShiftPlannerContext database) : base(database)
	{
	}
}