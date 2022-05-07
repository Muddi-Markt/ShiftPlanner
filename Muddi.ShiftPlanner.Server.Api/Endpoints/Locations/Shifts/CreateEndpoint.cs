using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Server.Database.Entities;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Locations.Shifts;

public class AddShiftEndpoint : CrudEndpoint<CreateLocationsShiftRequest, EmptyResponse>
{
	public AddShiftEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Post("/locations/{Id:guid}/shifts");
	}

	public override async Task HandleAsync(CreateLocationsShiftRequest req, CancellationToken ct)
	{
		var container = await Database.Containers
			.Include(c => c.Shifts)
			.Include(c => c.Framework)
			.ThenInclude(f => f.ShiftTypeCounts)
			.FirstOrDefaultAsync(t => req.Start >= t.Start && req.Start < t.End, cancellationToken: ct);
		if (container is null)
		{
			await SendNotFoundAsync("Container within time range");
			return;
		}

		var endTime = req.Start + container.Framework.TimePerShift;
		var type = new ShiftType { Id = req.ShiftTypeId };

		var shift = new Shift
		{
			Id = Guid.NewGuid(),
			EmployeeKeycloakId = req.EmployeeKeycloakId,
			Start = req.Start,
			End = endTime,
			Type = type
		};

		Database.ShiftTypes.Attach(type);
		container.Shifts.Add(shift);
		Database.Shifts.Add(shift);
		await Database.SaveChangesAsync(ct);
	}
}