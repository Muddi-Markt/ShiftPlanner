using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.ShiftLocations;

public class GetAllEndpoint : MuddiEndpointWithoutRequest<GetAllLocationsResponse>
{
	public GetAllEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void MuddiConfigure()
	{
		Get("/locations");
	}

	public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
	{
		var res = await Database.ShiftLocations.Include(t => t.Type).ToArrayAsync(cancellationToken: ct);
		Response.Locations = res.Select(t => t.ToResponse());
	}
}

public class GetAllLocationsResponse
{
	public IEnumerable<GetLocationResponse> Locations { get; set; }
}