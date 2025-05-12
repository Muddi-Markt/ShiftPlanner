using FastEndpoints;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Api.Extensions;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Shared.Contracts.v1;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Locations;

public class GetAllEndpoint : CrudGetAllEndpoint<GetLocationRequest, GetLocationResponse>
{
	public GetAllEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Roles(ApiRoles.Editor, ApiRoles.Viewer);
		Get("/locations");
	}

	public override async Task<List<GetLocationResponse>?> CrudExecuteAsync(GetLocationRequest req,
		CancellationToken ct) =>
		await Database.ShiftLocations
			.CheckAdminOnly(User)
			.Include(l => l.Type)
			.Where(l => l.Season.Id == req.SeasonId)
			.OrderBy(l => l.Containers.Min(x => x.Start)) //First order by starting time
			.ThenBy(l => l.Name) //Then by name
			.Select(t => t.Adapt<GetLocationResponse>())
			.ToListAsync(cancellationToken: ct);
}