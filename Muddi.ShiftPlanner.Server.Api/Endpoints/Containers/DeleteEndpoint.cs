using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Shared.Contracts.v1;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Containers;

public class DeleteEndpoint : CrudDeleteEndpoint
{
	public DeleteEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Roles(ApiRoles.Admin);
		Delete("/containers/{Id}");
	}

	protected override async Task<DeleteResponse> CrudExecuteAsync(Guid id, CancellationToken ct)
	{
		var entity = await Database.Containers.Include(c => c.Shifts)
			.FirstOrDefaultAsync(c => c.Id == id, cancellationToken: ct);
		if (entity is null)
			return DeleteResponse.NotFound;
		if (!User.IsInRole(ApiRoles.SuperAdmin))
		{
			var shiftsCount = entity.Shifts.Count;
			if (shiftsCount > 0)
			{
				await Send.ForbiddenAsync(
					"There are shifts attached to this container. Only a super-admin is allowed to delete this", ct);
				return DeleteResponse.Other;
			}
		}

		Database.RemoveRange(entity.Shifts);
		Database.Remove(entity);
		await Database.SaveChangesAsync(ct);
		return DeleteResponse.OK;
	}
}