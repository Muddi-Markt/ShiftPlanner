using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses.Frameworks;

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

	public override async Task<GetFrameworkResponse?> MuddiExecuteAsync(Guid id, CancellationToken ct)
		=> (await Database.ShiftFrameworks
				.Include(t => t.ShiftTypeCounts)
				.FirstOrDefaultAsync(l => l.Id == id, cancellationToken: ct))
			?.Adapt<GetFrameworkResponse>();
}