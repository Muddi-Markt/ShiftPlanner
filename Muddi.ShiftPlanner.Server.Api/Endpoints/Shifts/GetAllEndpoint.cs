using FastEndpoints;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Server.Database.Entities;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Shifts;

public class GetAllEndpoint : CrudGetAllEndpoint<GetAllShiftsRequest, GetShiftResponse>
{
	protected override void CrudConfigure()
	{
		Get("/shifts");
	}

	public override async Task<List<GetShiftResponse>?> CrudExecuteAsync(GetAllShiftsRequest request, CancellationToken ct)
	{
		IQueryable<ShiftEntity> query = Database.Shifts
			.Include(s => s.Type)
			.Include(s => s.ShiftContainer)
			.OrderBy(s => s.Start);
		if (request.Limit > 0)
		{
			query = query.Take(request.Limit.Value);
		}

		var res = await query
			.Select(t => t.Adapt<GetShiftResponse>())
			.ToListAsync(ct);
		return res;
	}

	public GetAllEndpoint(ShiftPlannerContext database) : base(database)
	{
	}
}