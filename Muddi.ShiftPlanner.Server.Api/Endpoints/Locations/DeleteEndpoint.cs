using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Shared.Contracts.v1;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Locations;

public class DeleteEndpoint : CrudDeleteEndpoint
{
	public DeleteEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Roles(ApiRoles.SuperAdmin);
		Delete("/locations/{Id}");
	}

	protected override async Task<DeleteResponse> CrudExecuteAsync(Guid id, CancellationToken ct)
	{
		var entity = await Database.ShiftLocations
			.Include(t => t.Containers)
			.ThenInclude(t => t.Shifts)
			.FirstOrDefaultAsync(t => t.Id == id, cancellationToken: ct);
		if (entity is null)
			return DeleteResponse.NotFound;
		Database.RemoveRange(entity.Containers.SelectMany(q => q.Shifts));
		Database.RemoveRange(entity.Containers);
		Database.Remove(entity);
		//TODO remove all corresponding container
		await Database.SaveChangesAsync(ct);
		return DeleteResponse.OK;
	}
}