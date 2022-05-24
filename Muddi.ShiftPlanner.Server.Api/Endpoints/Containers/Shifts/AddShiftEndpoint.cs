using System.Diagnostics.CodeAnalysis;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Api.Extensions;
using Muddi.ShiftPlanner.Server.Api.Services;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Server.Database.Entities;
using Muddi.ShiftPlanner.Shared;
using Muddi.ShiftPlanner.Shared.Contracts.v1;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Containers.Shifts;

public class AddShiftEndpoint : CrudEndpoint<CreateShiftRequest, DefaultCreateResponse>
{
	public AddShiftEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Roles(ApiRoles.Editor);
		Post("/containers/{Id:guid}/shifts");
	}

	public override async Task HandleAsync(CreateShiftRequest req, CancellationToken ct)
	{
		var container = await Database.Containers
			.Include(c => c.Shifts)
			.Include(c => c.Location)
			.Include(c => c.Framework)
			.ThenInclude(f => f.ShiftTypeCounts)
			.ThenInclude(stc => stc.ShiftType)
			.AsSingleQuery()
			.FirstOrDefaultAsync(t => t.Id == req.Id && req.Start >= t.Start && req.Start < t.End, cancellationToken: ct);


		if (container is null)
		{
			await SendNotFoundAsync("Container");
			return;
		}

		var failure = container.PreAddShiftSanityCheck(req);
		
		if (await SendErrorIfValidationFailure(failure))
			return;

		var shift = Database.AddShiftToContainer(req, container);

		await Database.SaveChangesAsync(ct);
		await SendCreatedAtAsync<Endpoints.Shifts.GetEndpoint>(new { shift.Id }, new() { Id = shift.Id }, cancellation: ct);
	}
}