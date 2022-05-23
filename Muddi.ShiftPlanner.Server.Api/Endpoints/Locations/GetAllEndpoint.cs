using FastEndpoints;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Shared.Contracts.v1;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Locations;

public class GetAllEndpoint : CrudGetAllEndpointWithoutRequest<GetLocationResponse>
{
	public GetAllEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Roles(ApiRoles.Editor, ApiRoles.Viewer);
		Get("/locations");
	}

	public override async Task<List<GetLocationResponse>> CrudExecuteAsync(EmptyRequest _, CancellationToken ct) =>
		await Database.ShiftLocations
			.Include(l => l.Type)
			.Select(t => t.Adapt<GetLocationResponse>())
			.AsNoTracking()
			.ToListAsync(cancellationToken: ct);
}