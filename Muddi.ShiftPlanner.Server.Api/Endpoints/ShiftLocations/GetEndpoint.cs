using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Server.Database.Entities;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.ShiftLocations;

public class GetEndpoint : MuddiEndpoint<GetLocationByIdRequest, GetLocationResponse>
{
	public GetEndpoint(ShiftPlannerContext database) : base(database)
	{
	}
	protected override void MuddiConfigure()
	{
		Get("/locations/{Id:guid}");
	}
	public override async Task HandleAsync(GetLocationByIdRequest req, CancellationToken ct)
	{
		var location = await Database.ShiftLocations.Include(t => t.Type).SingleOrDefaultAsync(l => l.Id == req.Id, cancellationToken: ct);
		if (location is null)
		{
			await SendNoContentAsync(ct);
			return;
		}

		Response = location.ToResponse();
	}
}

public class GetLocationByIdRequest
{
	public Guid Id { get; set; }
}

public class GetLocationResponse
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public ShiftLocationType Type { get; set; }
	public IEnumerable<ShiftContainer> Containers { get; set; }
}

public static class Mapper
{
	public static GetLocationResponse ToResponse(this Database.Entities.ShiftLocation location)
	{
		return new GetLocationResponse()
		{
			Id = location.Id,
			Name = location.Name,
			Type = location.Type,
			Containers = location.Containers
		};
	}
}