using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Api.Contracts.Requests;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Server.Database.Entities;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.ShiftLocations;

public class CreateEndpoint : MuddiEndpoint<CreateLocationRequest, GetLocationResponse>
{
	public CreateEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void MuddiConfigure()
	{
		Post("/locations");
	}

	public override async Task HandleAsync(CreateLocationRequest req, CancellationToken ct)
	{
		var type = await Database.ShiftLocationTypes.SingleOrDefaultAsync(t => t.Id == req.TypeId, ct);
		if (type is null)
		{
			await SendNotFoundAsync(nameof(req.TypeId));
			return;
		}

		var location = new ShiftLocation
		{
			Id = Guid.NewGuid(),
			Name = req.Name,
			Type = type,
			Containers = new List<ShiftContainer>()
		};
		Database.Add(location);
		await Database.SaveChangesAsync(ct);

		await SendCreatedAtAsync<GetEndpoint>(new { location.Id }, location.ToResponse(), cancellation: ct);
	}
}

public class CreateLocationRequest
{
	public string Name { get; set; }
	public Guid TypeId { get; set; }
}