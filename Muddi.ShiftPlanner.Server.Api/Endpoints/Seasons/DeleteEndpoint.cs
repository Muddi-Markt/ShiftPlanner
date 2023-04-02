using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Shared.Contracts.v1;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Seasons;

public class DeleteEndpoint : CrudDeleteEndpoint
{
	public DeleteEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Roles(ApiRoles.SuperAdmin);
		Delete("/seasons/{Id}");
	}

	protected override async Task<DeleteResponse> CrudExecuteAsync(Guid id, CancellationToken ct)
	{
		var entity = await Database.Seasons.FindAsync(new object?[] { id }, cancellationToken: ct);
		if (entity is null)
			return DeleteResponse.NotFound;
		//TODO remove all corresponding container
		Database.Remove(entity);
		await Database.SaveChangesAsync(ct);
		return DeleteResponse.OK;
	}
}