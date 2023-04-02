using FastEndpoints;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Api.Services;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Server.Database.Entities;
using Muddi.ShiftPlanner.Shared.Contracts.v1;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Shifts;

public class GetAllAvailableEndpoint : CrudGetAllEndpoint<GetAllShiftsRequest, GetShiftTypesCountResponse>
{
	protected override void CrudConfigure()
	{
		Roles(ApiRoles.Editor, ApiRoles.Viewer);
		Get("/shifts/available-types");
	}

	public override async Task<List<GetShiftTypesCountResponse>?> CrudExecuteAsync(GetAllShiftsRequest request, CancellationToken ct)
	{
		var locations = await Database.ShiftLocations
			.Include(l => l.Containers)
			.ThenInclude(c => c.Framework)
			.ThenInclude(f => f.ShiftTypeCounts)
			.ThenInclude(stc => stc.ShiftType)
			.Include(l => l.Containers)
			.ThenInclude(c => c.Shifts)
			.ThenInclude(s => s.Type)
			.AsSingleQuery()
			.AsNoTracking()
			.ToListAsync(cancellationToken: ct);
		if (locations.Count == 0)
		{
			await SendNoContent();
			return null;
		}

		bool isAdmin = User.IsInRole(ApiRoles.Admin);
		var startingFrom = request.StartingFrom?.ToUniversalTime();

		IEnumerable<GetShiftTypesCountResponse> shifts =
			locations.SelectMany(l => l.Containers
					.SelectMany(container => container.GetStartTimes()
						.SelectMany(container.GetAvailableShiftTypes)))
				.Where(s => s.AvailableCount > 0 && (s.Type.OnlyAssignableByAdmin == false || isAdmin))
				.OrderBy(st => st.Start);

		if (startingFrom is not null)
			shifts = shifts.Where(s => s.Start >= startingFrom);
		if (request.Limit > 0)
			shifts = shifts.Take(request.Limit.Value);

		return shifts.ToList();
	}

	public GetAllAvailableEndpoint(ShiftPlannerContext database) : base(database)
	{
	}
}