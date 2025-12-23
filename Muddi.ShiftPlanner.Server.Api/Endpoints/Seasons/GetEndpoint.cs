using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses.Seasons;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Seasons;

public class GetEndpoint : CrudGetEndpoint<GetSeasonResponse>
{
	public GetEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Roles(ApiRoles.Viewer);
		Get("/seasons/{Id:guid}");
	}

	public override async Task<GetSeasonResponse?> CrudExecuteAsync(Guid id, CancellationToken ct)
		=> (await Database.Seasons.FindAsync(id))?.Adapt<GetSeasonResponse>();
}