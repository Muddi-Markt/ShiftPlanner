using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Shared;
using Muddi.ShiftPlanner.Shared.Contracts.v1;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Shifts;

public class DeleteEndpoint : CrudDeleteEndpoint
{
	public DeleteEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Roles(ApiRoles.Editor);
		Delete("/shifts/{Id:guid}");
	}

	protected override async Task<DeleteResponse> CrudExecuteAsync(Guid id, CancellationToken ct)
	{
		var entity = await Database.Shifts.FindAsync(new object?[] { id }, cancellationToken: ct);
		if (entity is null)
			return DeleteResponse.NotFound;

		if (User.GetKeycloakId() != entity.EmployeeKeycloakId)
		{
			await SendForbiddenAsync(ct);
			return DeleteResponse.Other;
		}

		//Don't allow user to remove shifts after a certain time
		//TODO This needs to be set via Api by an admin in the future
		if (DateTime.UtcNow >= entity.Start.Date.AddDays(-1).AddHours(16))
		{
			await SendLockedAsync("You are not allowed to remove the shift anymore. Ask an admin.");
			return DeleteResponse.Other;
		}

		Database.Remove(entity);
		await Database.SaveChangesAsync(ct);
		return DeleteResponse.OK;
	}
}