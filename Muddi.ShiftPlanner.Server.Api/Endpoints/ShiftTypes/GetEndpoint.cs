using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses.Frameworks;

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

public class GetShiftTypesResponse : IMuddiResponse
{
	public Guid Id { get; set; }
	public string Name { get; set; }
}