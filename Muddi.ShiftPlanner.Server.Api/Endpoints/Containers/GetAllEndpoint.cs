using FastEndpoints;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Api.Extensions;
using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Containers;

public class GetAllEndpoint : CrudGetAllEndpoint<GetContainerRequest, GetContainerResponse>
{
	public GetAllEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Get("/containers");
	}

	public override async Task<List<GetContainerResponse>?> CrudExecuteAsync(GetContainerRequest req, CancellationToken ct)
	{
		return await Database.ShiftLocations
			.CheckAdminOnly(User)
			.Include(t => t.Type)
			.Where(q => q.Season.Id == req.SeasonId)
			.OrderBy(sl => sl.Containers.Min(c => c.Start))
			.Select(t => t.Adapt<GetContainerResponse>())
			.ToListAsync(cancellationToken: ct);
	}
}