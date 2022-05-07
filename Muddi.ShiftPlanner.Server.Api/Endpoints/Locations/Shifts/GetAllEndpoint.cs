using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Locations.Shifts;

public class GetAllEndpoint : CrudGetAllEndpoint<DefaultGetRequest, GetShiftResponse>
{
	public GetAllEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Get("/locations/{Id:guid}/shifts");
	}

	public override async Task<List<GetShiftResponse>> CrudExecuteAsync(DefaultGetRequest req, CancellationToken ct)
	{
		var location = await Database.ShiftLocations
			.Include(l => l.Containers)
			.ThenInclude(c => c.Shifts)
			.ThenInclude(s => s.Type)
			.FirstAsync(l => l.Id == req.Id, cancellationToken: ct);

		return location.Containers.SelectMany(c => c.Shifts.Select(s => s.Adapt<GetShiftResponse>())).ToList();
	}
}