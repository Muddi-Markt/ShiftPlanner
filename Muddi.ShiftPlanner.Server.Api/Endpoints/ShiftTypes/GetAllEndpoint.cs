using FastEndpoints;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.ShiftTypes;

public class GetAllEndpoint : CrudGetAllEndpoint<GetShiftTypesRequest, GetShiftTypesResponse>
{
	public GetAllEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Get("shift-types");
	}

	public override async Task<List<GetShiftTypesResponse>?> CrudExecuteAsync(GetShiftTypesRequest req, CancellationToken ct)
	{
		return await Database.ShiftTypes
			.Where(x => x.Season.Id == req.SeasonId)
			.OrderBy(st => st.Name)
			.Select(t => t.Adapt<GetShiftTypesResponse>())
			.ToListAsync(ct);
	}
}