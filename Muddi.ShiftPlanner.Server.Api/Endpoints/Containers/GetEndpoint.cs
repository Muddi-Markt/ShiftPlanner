using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses.Containers;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Containers;

public class GetEndpoint : CrudGetEndpoint<GetContainerResponse>
{
	public GetEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Get("/containers/{Id:guid}");
	}

	public override async Task<GetContainerResponse?> MuddiExecuteAsync(Guid id, CancellationToken ct)
		=> (await Database.Containers
				.Include(t => t.ShiftFramework)
				.FirstOrDefaultAsync(l => l.Id == id, cancellationToken: ct))
			?.Adapt<GetContainerResponse>();
}