using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Containers;

public class DeleteEndpoint : CrudDeleteEndpoint
{
	public DeleteEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Delete("/containers/{Id}");
	}

	public override async Task<bool> CrudExecuteAsync(Guid id, CancellationToken ct)
	{
		var entity = await Database.Containers.FindAsync(new object?[] { id }, cancellationToken: ct);
		if (entity is null)
			return false;
		Database.Remove(entity);
		await Database.SaveChangesAsync(ct);
		return true;
	}
}