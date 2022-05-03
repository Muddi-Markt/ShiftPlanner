using FastEndpoints;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Api.Contracts.Responses;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses.Containers;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Containers;

public class GetAllEndpoint : CrudGetAllEndpoint<GetContainerResponse>
{
	public GetAllEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Get("/containers");
	}

	public override async Task<ICollection<GetContainerResponse>> CrudExecuteAsync(CancellationToken ct)
	{
		return await Database.ShiftLocations
			.Include(t => t.Type)
			.Select(t => t.Adapt<GetContainerResponse>())
			.ToArrayAsync(cancellationToken: ct);
	}
}