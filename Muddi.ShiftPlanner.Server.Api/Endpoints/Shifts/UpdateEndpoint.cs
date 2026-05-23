using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Api.Services;
using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Shifts;

public class UpdateEndpoint : CrudUpdateEndpoint<CreateShiftRequest>
{
	public UpdateEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Roles(ApiRoles.Editor);
		Put("/shifts/{id}");
	}

	public override async Task CrudExecuteAsync(CreateShiftRequest request, CancellationToken ct)
	{
		var container = await Database.Containers
			.Include(c => c.Shifts)
			.ThenInclude(s => s.Type)
			.Include(c => c.Location)
			.Include(c => c.Framework)
			.ThenInclude(f => f.ShiftTypeCounts)
			.ThenInclude(stc => stc.ShiftType)
			.AsSingleQuery()
			.FirstOrDefaultAsync(c => c.Shifts.Any(s => s.Id == request.Id), cancellationToken: ct);
		if (container is null)
		{
			await Send.NotFoundAsync("Shift", ct);
			return;
		}

		var shift = container.Shifts.First(s => s.Id == request.Id);
		//Ignore this shift when checking if the user has a shift at the same time,
		//because we are updating this very shift. This applies regardless of whether
		//the shift type changes or not (e.g., admin blocking a shift without changing type).
		Guid ignoreUserHasShiftAtGivenTime = shift.EmployeeKeycloakId == request.EmployeeKeycloakId
		                                     && shift.Start == request.Start
			? shift.Type.Id
			: Guid.Empty;


		var failure = await Database.PreAddShiftSanityCheck(container, request, User, ignoreUserHasShiftAtGivenTime);
		if (await SendErrorIfValidationFailure(failure))
			return;


		shift.Start = request.Start.ToUniversalTime();
		if (shift.Type.Id != request.ShiftTypeId)
		{
			shift.Type = container.Framework.ShiftTypeCounts.First(x => x.ShiftType.Id == request.ShiftTypeId)
				.ShiftType;
		}

		shift.EmployeeKeycloakId = request.EmployeeKeycloakId;

		// Only admins are allowed to block/unblock shifts
		if (User.IsInRole(ApiRoles.Admin))
		{
			if (request.BlockReason?.Length > 50)
				throw new ArgumentException("Length is not allowed to be greater than 50 characters");
			shift.BlockReason = request.BlockReason;
		}
		else if (!string.IsNullOrEmpty(request.BlockReason))
		{
			await Send.ForbiddenAsync("Only administrators can block or unblock shifts.", ct);
			return;
		}

		Database.Shifts.Update(shift);
		await Database.SaveChangesAsync(ct);
	}
}