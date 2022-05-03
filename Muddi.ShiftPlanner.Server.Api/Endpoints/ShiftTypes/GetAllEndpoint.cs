using FastEndpoints;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Api.Contracts.Responses;
using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.ShiftTypes;

public class GetAllEndpoint : CrudGetAllEndpoint<GetShiftTypesResponse>
{
	public GetAllEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Get("/shift-types");
	}

	public override async Task<ICollection<GetShiftTypesResponse>> CrudExecuteAsync(CancellationToken ct)
	{
		return await Database.ShiftLocationTypes.Select(t => t.Adapt<GetShiftTypesResponse>()).ToArrayAsync(ct);
	}
}