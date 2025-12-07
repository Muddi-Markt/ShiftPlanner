using System.Data.SqlTypes;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Api.Services;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Shared.Contracts.v1;
using Namotion.Reflection;

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
		//If only the shift type is changed, ignore the ShiftType id when checking
		//whether the user has a same shift at the same time (as it is only this one,
		//which will be updated
		Guid ignoreUserHasShiftAtGivenTime = shift.EmployeeKeycloakId == request.EmployeeKeycloakId
		                                     && shift.Start == request.Start
		                                     && shift.Type.Id != request.ShiftTypeId
			? shift.Type.Id
			: default;


		var failure = await Database.PreAddShiftSanityCheck(container, request, User, ignoreUserHasShiftAtGivenTime);
		if (await SendErrorIfValidationFailure(failure))
			return;


		shift.Start = request.Start.ToUniversalTime();
		if (shift.Type.Id != request.ShiftTypeId)
		{
			shift.Type = container.Framework.ShiftTypeCounts.First(x => x.ShiftType.Id == request.ShiftTypeId).ShiftType;
		}

		shift.EmployeeKeycloakId = request.EmployeeKeycloakId;
		Database.Shifts.Update(shift);
		await Database.SaveChangesAsync(ct);
	}
}