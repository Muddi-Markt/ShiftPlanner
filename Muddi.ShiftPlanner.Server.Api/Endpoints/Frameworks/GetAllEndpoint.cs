using FastEndpoints;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Frameworks;

public class GetAllEndpoint : CrudGetAllEndpoint<GetFrameworkRequest,GetFrameworkResponse>
{
	public GetAllEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Get("/frameworks");
	}

	public override async Task<List<GetFrameworkResponse>> CrudExecuteAsync(GetFrameworkRequest req, CancellationToken ct)
	{
		return await Database.ShiftFrameworks
			.Include(t => t.ShiftTypeCounts)
			.ThenInclude(c => c.ShiftType)
			.Where(sf => sf.Season.Id == req.SeasonId)
			.Select(t => t.Adapt<GetFrameworkResponse>())
			.AsNoTracking()
			.ToListAsync(cancellationToken: ct);
	}
}