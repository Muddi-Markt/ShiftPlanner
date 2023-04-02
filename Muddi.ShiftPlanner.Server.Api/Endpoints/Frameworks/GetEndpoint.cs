using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Frameworks;

public class GetEndpoint : CrudGetEndpoint<GetFrameworkResponse>
{
	public GetEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Get("/frameworks/{Id:guid}");
	}

	public override async Task<GetFrameworkResponse?> CrudExecuteAsync(Guid id, CancellationToken ct)
		=> (await Database.ShiftFrameworks
				.Include(f => f.Season)
				.Include(t => t.ShiftTypeCounts)
				.ThenInclude(c => c.ShiftType)
				.AsNoTracking()
				.FirstOrDefaultAsync(l => l.Id == id, cancellationToken: ct))
			?.Adapt<GetFrameworkResponse>();
}