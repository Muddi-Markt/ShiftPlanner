using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses.LocationTypes;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.LocationTypes;

public class GetAllEndpoint : CrudGetAllEndpoint<GetLocationTypesResponse>
{
	protected override void CrudConfigure()
	{
		Get("/location-types");
	}

	public override async Task<ICollection<GetLocationTypesResponse>> CrudExecuteAsync(CancellationToken ct)
	{
		var res = await Database.ShiftLocationTypes.Select(t => t.Adapt<GetLocationTypesResponse>()).ToListAsync(ct);
		return res;
	}

	public GetAllEndpoint(ShiftPlannerContext database) : base(database)
	{
	}
}