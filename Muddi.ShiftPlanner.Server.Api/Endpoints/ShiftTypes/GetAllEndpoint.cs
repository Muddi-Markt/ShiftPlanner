using FastEndpoints;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.ShiftTypes;

public class GetAllEndpoint : CrudGetAllEndpointWithoutRequest<GetShiftTypesResponse>
{
	public GetAllEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Get("/shift-types");
	}

	public override async Task<List<GetShiftTypesResponse>> CrudExecuteAsync(EmptyRequest _, CancellationToken ct)
	{
		return await Database.ShiftTypes
			.Select(t => t.Adapt<GetShiftTypesResponse>())
			.ToListAsync(ct);
	}
}