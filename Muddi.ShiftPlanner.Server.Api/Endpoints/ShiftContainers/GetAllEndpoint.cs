using FastEndpoints;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Api.Endpoints.ShiftLocations;
using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.ShiftContainers;

public class GetAllEndpoint : MuddiEndpointWithoutRequest<GetAllContainersResponse>
{
	public GetAllEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void MuddiConfigure()
	{
		Get("/containers");
	}

	public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
	{
		var res = await Database.ShiftLocations.Include(t => t.Type).ToArrayAsync(cancellationToken: ct);
		Response.Containers = res.Select(t => t.Adapt<GetContainerResponse>());
	}
}

public class GetAllContainersResponse
{
	public IEnumerable<GetContainerResponse> Containers { get; set; }
}