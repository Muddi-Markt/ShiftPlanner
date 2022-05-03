using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses.Locations;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Locations;

public class GetAllEndpoint : CrudGetAllEndpoint<GetLocationResponse>
{
	public GetAllEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Get("/locations");
	}

	public override async Task<ICollection<GetLocationResponse>> CrudExecuteAsync(CancellationToken ct) =>
		await Database.ShiftLocations
			.Include(t => t.Type)
			.Include(t => t.Containers)
			.ThenInclude(c => c.ShiftFramework)
			.ThenInclude(f => f.ShiftTypeCounts)
			.Select(t => t.Adapt<GetLocationResponse>())
			.ToArrayAsync(cancellationToken: ct);
	
}