using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Api.Contracts.Requests;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Server.Database.Entities;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.ShiftFrameworks;

public class GetEndpoint : MuddiEndpoint<DefaultGetRequest, GetFrameworkResponse>
{
	public GetEndpoint(ShiftPlannerContext database) : base(database)
	{
	}
	protected override void MuddiConfigure()
	{
		Get("/frameworks/{Id:guid}");
	}
	public override async Task HandleAsync(DefaultGetRequest req, CancellationToken ct)
	{
		var framework = await Database.ShiftFrameworks.Include(t => t.ShiftTypeCounts).SingleOrDefaultAsync(l => l.Id == req.Id, cancellationToken: ct);
		if (framework is null)
		{
			await SendNotFoundAsync(nameof(req.Id));
			return;
		}
		

		Response = framework.Adapt<GetFrameworkResponse>();
	}
}

public class GetFrameworkResponse
{
	public Guid Id { get; set; }
	public int TimePerShift { get; set; }
	public IEnumerable<ShiftFrameworkTypeCountDto> ShiftTypeCounts { get; set; }
}

public class ShiftFrameworkTypeCountDto
{
	public Guid ShiftTypeId { get; set; }
	public int Count { get; set; }
}