using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Api.Contracts.Requests;
using Muddi.ShiftPlanner.Server.Api.Endpoints.ShiftFrameworks;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Server.Database.Entities;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.ShiftContainers;

public class GetEndpoint : MuddiEndpoint<DefaultGetRequest, GetContainerResponse>
{
	public GetEndpoint(ShiftPlannerContext database) : base(database)
	{
	}
	protected override void MuddiConfigure()
	{
		Get("/containers/{Id:guid}");
	}
	public override async Task HandleAsync(DefaultGetRequest req, CancellationToken ct)
	{
		var container = await Database.Containers.Include(t => t.ShiftFramework).SingleOrDefaultAsync(l => l.Id == req.Id, cancellationToken: ct);
		if (container is null)
		{
			await SendNotFoundAsync(nameof(req.Id));
			return;
		}
		

		Response = container.Adapt<GetContainerResponse>();
	}
}

public class GetContainerResponse
{
	public Guid Id { get; set; }
	public DateTime	Start { get; set; }
	public int TotalShifts { get; set; }
	public GetFrameworkResponse ShiftFramework { get; set; }
}