using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.ShiftTypes;

public class GetEndpoint : CrudGetEndpoint<GetShiftTypesResponse>
{
	public GetEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Get("/shift-types/{Id}");
	}

	public override async Task<GetShiftTypesResponse?> MuddiExecuteAsync(Guid id, CancellationToken ct)
		=> (await Database.ShiftTypes
				.FirstOrDefaultAsync(x => x.Id == id, cancellationToken: ct))
			?.Adapt<GetShiftTypesResponse>();
}