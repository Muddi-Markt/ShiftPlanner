using FastEndpoints;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Locations;

public class GetAllEndpoint : CrudGetAllEndpointWithoutRequest<GetLocationResponse>
{
	public GetAllEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Get("/locations");
	}

	public override async Task<List<GetLocationResponse>> CrudExecuteAsync(EmptyRequest _, CancellationToken ct) =>
		await Database.ShiftLocations
			.Include(l => l.Type)
			.Select(t => t.Adapt<GetLocationResponse>())
			.ToListAsync(cancellationToken: ct);
}