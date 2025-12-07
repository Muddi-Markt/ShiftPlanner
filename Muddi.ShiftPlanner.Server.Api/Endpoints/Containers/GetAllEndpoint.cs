using Mapster;
using Microsoft.EntityFrameworkCore;
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

	public override async Task<List<GetContainerResponse>?> CrudExecuteAsync(GetContainerRequest req,
		CancellationToken ct)
	{
		return await Database.Containers
			.Include(x => x.Framework)
			.ThenInclude(f => f.ShiftTypeCounts)
			.ThenInclude(stc => stc.ShiftType)
			.Where(q => q.Framework.Season.Id == req.SeasonId)
			.OrderBy(sl => sl.Start)
			.Select(t => t.Adapt<GetContainerResponse>())
			.ToListAsync(cancellationToken: ct);
	}
}