using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Server.Database.Entities;
using Muddi.ShiftPlanner.Shared.Contracts.v1;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Seasons;

public class UpdateEndpoint : CrudUpdateEndpoint<UpdateSeasonRequest>
{
	public UpdateEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Put("/seasons/{Id}");
	}

	public override async Task CrudExecuteAsync(UpdateSeasonRequest request, CancellationToken ct)
	{
		var entity = await Database.Seasons.FindAsync([request.Id], ct);
		if (entity is null)
		{
			await SendNotFoundAsync("season");
			return;
		}

		if (!(request.StartDate == default && request.EndDate == default))
		{
			bool isSuperAdmin = User.IsInRole(ApiRoles.SuperAdmin);
			if (!isSuperAdmin)
			{
				if (entity.StartDate != request.StartDate || entity.EndDate != request.EndDate)
				{
					await SendForbiddenAsync("Only super admins are allowed to edit start/end date of seasons");
					return;
				}
			}

			entity.StartDate = request.StartDate;
			entity.EndDate = request.EndDate;
		}

		entity.Name = request.Name;

		await Database.SaveChangesAsync(ct);
	}
}