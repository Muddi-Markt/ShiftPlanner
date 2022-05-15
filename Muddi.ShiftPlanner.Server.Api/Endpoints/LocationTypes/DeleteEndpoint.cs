using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.LocationTypes;

public class DeleteEndpoint : CrudDeleteEndpoint
{
	public DeleteEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Roles(ApiRoles.SuperAdmin);
		Delete("/location-types/{Id}");
	}

	protected override async Task<DeleteResponse> CrudExecuteAsync(Guid id, CancellationToken ct)
	{
		var entity = await Database.ShiftLocationTypes.FindAsync(new object?[] { id }, cancellationToken: ct);
		if (entity is null)
			return DeleteResponse.NotFound;
		Database.Remove(entity);
		//TODO delete all corresponding locations
		await Database.SaveChangesAsync(ct);
		return DeleteResponse.OK;
	}
}