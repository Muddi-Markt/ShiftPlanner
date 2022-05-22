using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Server.Database.Entities;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Locations;

public class AddContainerEndpoint : CrudEndpoint<AddLocationsContainerRequest, EmptyResponse>
{
	public AddContainerEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Put("/locations/{LocationId:guid}/container");
	}

	public override async Task HandleAsync(AddLocationsContainerRequest req, CancellationToken ct)
	{
		var location = await Database.ShiftLocations.Include(t => t.Containers).FirstOrDefaultAsync(t => t.Id == req.LocationId, ct);
		if (location == null)
		{
			await SendNotFoundAsync(ct);
			return;
		}

		foreach (var id in req.ContainerIds)
		{
			var c = new ShiftContainerEntity { Id = id };
			Database.Attach(c);
			location.Containers.Add(c);
		}

		await Database.SaveChangesAsync(ct);
	}
}