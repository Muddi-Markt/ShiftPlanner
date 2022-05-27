using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Shared.Contracts.v1;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Locations.Shifts;

public class GetShiftsCountEndpoint : CrudGetEndpoint<GetShiftsCountResponse>
{
	public GetShiftsCountEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Roles(ApiRoles.Editor, ApiRoles.Viewer);
		Get("/locations/{Id}/get-shifts-count");
	}

	public override async Task<GetShiftsCountResponse?> CrudExecuteAsync(Guid id, CancellationToken ct)
	{
		var total = await Database.ShiftLocations
			.Where(l => l.Id == id)
			.SumAsync(l => l.Containers
				.Sum(c => c.TotalShifts * c.Framework.ShiftTypeCounts
					.Sum(st => st.Count)), cancellationToken: ct);
		var assigned = await Database.Shifts.CountAsync(s => s.ShiftContainer.Location.Id == id, cancellationToken: ct);
		if (assigned > total)
		{
			Logger.LogWarning("Assigned {Ass} is more then total {Total} shifts for Location {Id}", assigned, total, id);
			assigned = total;
		}

		return new()
		{
			TotalShifts = total,
			AssignedShifts = assigned
		};
	}
}

public class GetShiftsCountResponse
{
	public int TotalShifts { get; set; }
	public int AssignedShifts { get; set; }
}