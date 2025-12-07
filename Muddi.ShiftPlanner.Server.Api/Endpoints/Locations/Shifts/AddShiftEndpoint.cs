using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Api.Extensions;
using Muddi.ShiftPlanner.Server.Api.Services;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Server.Database.Entities;
using Muddi.ShiftPlanner.Shared;
using Muddi.ShiftPlanner.Shared.Contracts.v1;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Locations.Shifts;

public class AddShiftEndpoint : CrudEndpoint<CreateShiftRequest, DefaultCreateResponse>
{
	public AddShiftEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Roles(ApiRoles.Editor);
		Post("/locations/{Id:guid}/shifts");
	}

	public override async Task HandleAsync(CreateShiftRequest req, CancellationToken ct)
	{
		var location = await Database.ShiftLocations
			.CheckAdminOnly(User)
			.Include(l => l.Containers.Where(t => req.Start >= t.Start && req.Start < t.End))
			.ThenInclude(c => c.Framework)
			.Include(l => l.Containers.Where(t => req.Start >= t.Start && req.Start < t.End))
			.ThenInclude(c => c.Shifts)
			.AsSingleQuery()
			.FirstOrDefaultAsync(l => l.Id == req.Id, cancellationToken: ct);
		if (location is null)
		{
			await Send.NotFoundAsync("Location", ct);
			return;
		}

		var container = location.Containers.SingleOrDefault();
		if (container is null)
		{
			await Send.NotFoundAsync("Container within time range", ct);
			return;
		}

		var failure = await Database.PreAddShiftSanityCheck(container, req, User);

		if (await SendErrorIfValidationFailure(failure))
			return;

		var shift = Database.AddShiftToContainer(req, container);
		await Send.CreatedAtAsync<Endpoints.Shifts.GetEndpoint>(new { shift.Id },
			new() { Id = shift.Id },
			cancellation: ct);
	}
}