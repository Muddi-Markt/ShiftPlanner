using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Shared.Contracts.v1;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.ShiftTypes;

public class DeleteEndpoint : CrudDeleteEndpoint
{
	public DeleteEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Roles(ApiRoles.SuperAdmin);
		Delete("/shift-types/{Id}");
	}

	protected override async Task<DeleteResponse> CrudExecuteAsync(Guid id, CancellationToken ct)
	{
		var entity = await Database.ShiftTypes.FindAsync(new object?[] { id }, cancellationToken: ct);
		if (entity is null)
			return DeleteResponse.NotFound;
		Database.Remove(entity);
		//TODO delete all corresponding Shifts
		await Database.SaveChangesAsync(ct);
		return DeleteResponse.OK;
	}
}