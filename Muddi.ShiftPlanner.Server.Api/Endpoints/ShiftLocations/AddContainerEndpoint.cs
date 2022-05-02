using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Server.Database.Entities;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.ShiftLocations;

public class AddContainerEndpoint : MuddiEndpoint<AddLocationsContainerRequest,AddLocationsContainerResponse>
{
	public AddContainerEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void MuddiConfigure()
	{
		Put("/locations/container/{LocationId:guid}");
	}

	public override async Task HandleAsync(AddLocationsContainerRequest req, CancellationToken ct)
	{
		var location = await Database.ShiftLocations.Include(t => t.Containers).SingleOrDefaultAsync(t => t.Id == req.LocationId, ct);
		if (location == null)
		{
			await SendNotFoundAsync(ct);
			return;
		}
		foreach (var id in req.ContainerIds)
		{
			var c = new ShiftContainer { Id = id };
			Database.Attach(c);
			location.Containers.Add(c);
		}
		
		await Database.SaveChangesAsync(ct);
	}
}

public class AddLocationsContainerResponse
{
}

public class AddLocationsContainerRequest
{
	public Guid LocationId { get; set; }
	public IEnumerable<Guid> ContainerIds { get; set; }
}