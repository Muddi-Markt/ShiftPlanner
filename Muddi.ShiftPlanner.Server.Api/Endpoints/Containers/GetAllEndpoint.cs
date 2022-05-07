using FastEndpoints;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Containers;

public class GetAllEndpoint : CrudGetAllEndpointWithoutRequest<GetContainerResponse>
{
	public GetAllEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Get("/containers");
	}

	public override async Task<List<GetContainerResponse>> CrudExecuteAsync(EmptyRequest _, CancellationToken ct)
	{
		return await Database.ShiftLocations
			.Include(t => t.Type)
			.Select(t => t.Adapt<GetContainerResponse>())
			.ToListAsync(cancellationToken: ct);
	}
}