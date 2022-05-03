using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses.Containers;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses.Frameworks;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Frameworks;

public class GetAllEndpoint : CrudGetAllEndpoint<GetFrameworkResponse>
{
	public GetAllEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Get("/frameworks");
	}

	public override async Task<ICollection<GetFrameworkResponse>> CrudExecuteAsync(CancellationToken ct)
	{
		return await Database.ShiftFrameworks
			.Include(t => t.ShiftTypeCounts)
			.Select(t => t.Adapt<GetFrameworkResponse>())
			.ToArrayAsync(cancellationToken: ct);
	}
}