using Mapster;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Server.Database.Entities;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.ShiftTypes;

public class CreateEndpoint : CrudCreateEndpoint<CreateShiftTypeRequest, GetShiftTypesResponse, GetEndpoint>
{
	public CreateEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Post("/shift-types");
	}

	public override async Task<GetShiftTypesResponse?> CrudExecuteAsync(CreateShiftTypeRequest req, CancellationToken ct)
	{
		var type = new ShiftType
		{
			Id = Guid.NewGuid(),
			Name = req.Name,
		};
		Database.Add(type);
		await Database.SaveChangesAsync(ct);
		return type.Adapt<GetShiftTypesResponse>();
	}
}