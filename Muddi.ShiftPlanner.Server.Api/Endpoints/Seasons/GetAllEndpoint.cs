using FastEndpoints;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses.Seasons;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Seasons;

public class GetAllEndpoint : CrudGetAllEndpointWithoutRequest<GetSeasonResponse>
{
	public GetAllEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Roles(ApiRoles.Viewer);
		Get("/seasons");
	}

	public override async Task<List<GetSeasonResponse>?> CrudExecuteAsync(EmptyRequest _, CancellationToken ct)
	{
		return await Database.Seasons
			.OrderBy(s => s.StartDate)
			.Select(t => t.Adapt<GetSeasonResponse>())
			.ToListAsync(cancellationToken: ct);
	}
}