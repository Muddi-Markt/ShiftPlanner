using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Api.Services;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Shared.Contracts.v1;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Locations.Shifts;

public class GetAllAvailableShiftTypesEndpoint : CrudGetAllEndpoint<GetAvailableShiftsForLocationRequest, GetShiftTypesCountResponse>
{
	public GetAllAvailableShiftTypesEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Roles(ApiRoles.Editor, ApiRoles.Viewer);
		Get("/locations/{LocationId}/get-all-available-shifts-types");
	}

	public override async Task<List<GetShiftTypesCountResponse>?> CrudExecuteAsync(GetAvailableShiftsForLocationRequest request, CancellationToken ct)
	{
		var start = request.StartTime?.ToUniversalTime() ?? DateTime.UnixEpoch;
		var end = request.EndTime?.ToUniversalTime() ?? DateTime.MaxValue;
		var location = await Database.ShiftLocations
			.Include(l => l.Containers.Where(c => c.Start >= start && c.Start < end))
			.ThenInclude(c => c.Framework)
			.ThenInclude(f => f.ShiftTypeCounts)
			.ThenInclude(stc => stc.ShiftType)
			.Include(l => l.Containers.Where(c => c.Start >= start && c.Start < end))
			.ThenInclude(c => c.Shifts)
			.ThenInclude(s => s.Type)
			.AsSingleQuery()
			.AsNoTracking()
			.FirstOrDefaultAsync(l => l.Id == request.LocationId, cancellationToken: ct);
		if (location is null)
		{
			await SendNotFoundAsync("location");
			return null;
		}


		var shifts = location.Containers
			.SelectMany(container => container.GetStartTimes()
				.SelectMany(container.GetAvailableShiftTypes));

		return shifts.ToList();
	}
}