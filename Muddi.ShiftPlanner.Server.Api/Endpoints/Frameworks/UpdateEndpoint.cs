using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Server.Database.Entities;
using Muddi.ShiftPlanner.Shared.Contracts.v1;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Frameworks;

public class UpdateEndpoint : CrudUpdateEndpoint<UpdateFrameworkRequest>
{
	public UpdateEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Put("/frameworks/{Id}");
	}

	public override async Task CrudExecuteAsync(UpdateFrameworkRequest request, CancellationToken ct)
	{
		var entity = await Database.ShiftFrameworks
			.Include(f => f.Season)
			.Include(f => f.ShiftTypeCounts)
			.ThenInclude(s => s.ShiftType)
			.FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken: ct);
		if (entity is null)
		{
			await SendNotFoundAsync("framework");
			return;
		}

		bool isSuperAdmin = User.IsInRole(ApiRoles.SuperAdmin);
		if (!isSuperAdmin)
		{
			if (entity.SecondsPerShift != request.SecondsPerShift)
			{
				await SendForbiddenAsync("Only super admins are allowed to change time per shift");
				return;
			}

			if (entity.ShiftTypeCounts.Count > request.TypeCounts.Count)
			{
				await SendForbiddenAsync("Only super admins are allowed to remove shift types");
				return;
			}

			foreach (var stc in entity.ShiftTypeCounts)
			{
				var tc = request.TypeCounts.FirstOrDefault(t => t.ShiftTypeId == stc.ShiftType.Id);
				if (tc is null)
				{
					await SendForbiddenAsync("Only super admins are allowed to remove single shift type");
					return;
				}

				if (stc.Count > tc.Count)
				{
					await SendForbiddenAsync("Only super admins are allowed to decrease shift type counts");
					return;
				}
			}
		}

		entity.Name = request.Name;

		if (entity.Season.Id != request.SeasonId)
			entity.Season = new() { Id = request.SeasonId };

		var typesToAdd = request.TypeCounts.ToList();
		entity.ShiftTypeCounts.RemoveAll(q => request.TypeCounts.All(tc => tc.ShiftTypeId != q.ShiftType.Id));
		foreach (var stc in entity.ShiftTypeCounts)
		{
			var tc = request.TypeCounts.First(t => t.ShiftTypeId == stc.ShiftType.Id);
			stc.Count = tc.Count;
			typesToAdd.Remove(tc);
		}

		foreach (var tc in typesToAdd)
		{
			var stcEntity = new ShiftFrameworkTypeCountEntity
			{
				Id = Guid.NewGuid(),
				ShiftFramework = entity,
				Count = tc.Count,
				ShiftType = new() { Id = tc.ShiftTypeId }
			};
			entity.ShiftTypeCounts.Add(stcEntity);

			Database.Add(stcEntity);
			Database.Attach(stcEntity.ShiftType);
		}

		entity.SecondsPerShift = request.SecondsPerShift;
		await Database.SaveChangesAsync(ct);
	}
}