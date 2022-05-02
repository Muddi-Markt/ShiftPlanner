using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Server.Database.Entities;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.ShiftFrameworks;

public class CreateEndpoint : MuddiEndpoint<CreateFrameworkRequest, GetFrameworkResponse>
{
	public CreateEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void MuddiConfigure()
	{
		Post("/frameworks");
	}

	public override async Task HandleAsync(CreateFrameworkRequest req, CancellationToken ct)
	{
		var framework = new ShiftFramework
		{
			Id = Guid.NewGuid(),
			TimePerShift = req.TimePerShift,
			ShiftTypeCounts = new List<ShiftFrameworkTypeCount>()
		};
		framework.ShiftTypeCounts.AddRange(req.TypeCounts.Select(t => new ShiftFrameworkTypeCount()
		{
			Id = Guid.NewGuid(),
			Count = t.Count,
			ShiftFrameworkId = framework.Id,
			ShiftTypeId = t.ShiftTypeId
		}));

		Database.ShiftFrameworks.Add(framework);
		await Database.SaveChangesAsync(ct);
		await SendCreatedAtAsync<GetEndpoint>(new { framework.Id }, framework.Adapt<GetFrameworkResponse>(), cancellation: ct);
	}
}

public class CreateFrameworkRequest
{
	public int TimePerShift { get; set; }
	public IList<ShiftFrameworkTypeCountDto> TypeCounts { get; set; }
}