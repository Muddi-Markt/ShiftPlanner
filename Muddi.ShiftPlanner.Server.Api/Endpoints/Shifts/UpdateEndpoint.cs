using System.Data.SqlTypes;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Shared.Contracts.v1;

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
		var shift = await Database.Shifts.Include(s => s.Type).FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken: ct);
		if (shift is null)
		{
			await SendNotFoundAsync("Shift");
			return;
		}

		shift.Start = request.Start.ToUniversalTime();
		if (shift.Type.Id != request.ShiftTypeId)
		{
			shift.Type = new() { Id = request.ShiftTypeId };
			Database.ShiftTypes.Attach(shift.Type);
		}

		shift.EmployeeKeycloakId = request.EmployeeKeycloakId;
		Database.Shifts.Update(shift);
		await Database.SaveChangesAsync(ct);
	}
}