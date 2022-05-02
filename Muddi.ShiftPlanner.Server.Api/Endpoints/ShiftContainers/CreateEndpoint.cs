using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Api.Contracts.Requests;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Server.Database.Entities;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.ShiftContainers;

public class CreateEndpoint : MuddiEndpoint<CreateContainerRequest, GetContainerResponse>
{
	public CreateEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void MuddiConfigure()
	{
		Post("/containers");
	}

	public override async Task HandleAsync(CreateContainerRequest req, CancellationToken ct)
	{
		var framework = await Database.ShiftFrameworks.Include(t => t.ShiftTypeCounts).SingleOrDefaultAsync(t => t.Id == req.FrameworkId, cancellationToken: ct);
		if (framework is null)
		{
			await SendNotFoundAsync(nameof(req.FrameworkId));
			return;
		}

		var container = new ShiftContainer
		{
			Id = Guid.NewGuid(),
			Start = req.Start,
			TotalShifts = req.TotalShifts,
			ShiftFramework = framework
		};

		Database.Containers.Add(container);
		await Database.SaveChangesAsync(ct);
		await SendCreatedAtAsync<GetEndpoint>(new { container.Id }, container.Adapt<GetContainerResponse>(), cancellation: ct);
	}
}

public class CreateContainerRequest
{
	public Guid FrameworkId { get; set; }
	public DateTime Start { get; set; }
	public int TotalShifts { get; set; }
}