using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Api.Endpoints.ShiftLocations;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Server.Database.Entities;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.ShiftLocationTypes;

public class GetAllEndpoint : MuddiEndpointWithoutRequest<GetAllLocationTypesResponse>
{
	public GetAllEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void MuddiConfigure()
	{
		Get("/location-types");
	}

	public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
	{
		var res = await Database.ShiftLocationTypes.ToArrayAsync(cancellationToken: ct);
		Response.LocationTypes = res;
	}
}

public class GetAllLocationTypesResponse
{
	public IEnumerable<ShiftLocationType> LocationTypes { get; set; }
}