using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Api.Services;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Shared.Contracts.v1;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Containers;

public class GetAvailableShiftTypesEndpoint : CrudGetAllEndpoint<GetAvailableShiftTypesRequest, GetShiftTypesCountResponse>
{
	public GetAvailableShiftTypesEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Roles(ApiRoles.Editor, ApiRoles.Viewer);
		Get("/containers/{ContainerId}/get-available-shift-types");
	}

	public override async Task<List<GetShiftTypesCountResponse>?> CrudExecuteAsync(GetAvailableShiftTypesRequest request, CancellationToken ct)
	{
		var container = await Database.Containers
			.Include(c => c.Framework)
			.ThenInclude(f => f.ShiftTypeCounts)
			.ThenInclude(stc => stc.ShiftType)
			.Include(c => c.Shifts)
			.ThenInclude(s => s.Type)
			.AsNoTracking()
			.FirstOrDefaultAsync(c => c.Id == request.ContainerId, cancellationToken: ct);
		if (container is null)
		{
			await SendNotFoundAsync("container");
			return null;
		}

		return container.GetAvailableShiftTypes(request.StartTime).ToList();
	}
}

public class GetAvailableShiftTypesRequest
{
	public Guid ContainerId { get; set; }
	[BindFrom("start")] public DateTime StartTime { get; set; }
}