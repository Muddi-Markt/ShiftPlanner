using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Shared.Contracts.v1;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Locations;

public class GetEndpoint : CrudGetEndpoint<GetLocationResponse>
{
	public GetEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Roles(ApiRoles.Editor, ApiRoles.Viewer);
		Get("/locations/{Id:guid}");
	}

	public override async Task<GetLocationResponse?> CrudExecuteAsync(Guid id, CancellationToken ct) =>
		(await Database.ShiftLocations
			.Include(t => t.Type)
			.Include(t => t.Containers)
			.ThenInclude(c => c.Framework)
			.ThenInclude(f => f.ShiftTypeCounts)
			.ThenInclude(c => c.ShiftType)
			.AsSingleQuery()
			.FirstOrDefaultAsync(l => l.Id == id, cancellationToken: ct))
		?.Adapt<GetLocationResponse>();
}