using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Locations;

public class DeleteEndpoint : CrudDeleteEndpoint
{
	public DeleteEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Delete("/locations/{Id}");
	}

	public override async Task<bool> CrudExecuteAsync(Guid id, CancellationToken ct)
	{
		var entity = await Database.ShiftLocations.FindAsync(new object?[] { id }, cancellationToken: ct);
		if (entity is null)
			return false;
		Database.Remove(entity);
		await Database.SaveChangesAsync(ct);
		return true;
	}
}