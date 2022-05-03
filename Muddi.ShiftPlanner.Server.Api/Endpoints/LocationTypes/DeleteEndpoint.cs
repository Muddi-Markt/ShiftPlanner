using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.LocationTypes;

public class DeleteEndpoint : CrudDeleteEndpoint
{
	public DeleteEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Delete("/location-types/{Id}");
	}

	public override async Task<bool> CrudExecuteAsync(Guid id, CancellationToken ct)
	{
		var entity = await Database.ShiftLocationTypes.FindAsync(new object?[] { id }, cancellationToken: ct);
		if (entity is null)
			return false;
		Database.Remove(entity);
		await Database.SaveChangesAsync(ct);
		return true;
	}
}