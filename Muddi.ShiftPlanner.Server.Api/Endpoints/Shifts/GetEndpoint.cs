using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Api.Services;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Shared.Contracts.v1;
using Refit;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Shifts;

public class GetEndpoint : CrudGetEndpoint<GetShiftResponse>
{
	private readonly IKeycloakService _keycloakService;

	public GetEndpoint(ShiftPlannerContext database ,IKeycloakService keycloakService) : base(database)
	{
		_keycloakService = keycloakService;
	}

	protected override void CrudConfigure()
	{
		Roles(ApiRoles.Editor, ApiRoles.Viewer);
		Get("/shifts/{id}");
	}

	public override async Task<GetShiftResponse?> CrudExecuteAsync(Guid id, CancellationToken ct)
	{
		var entity = await Database.Shifts.Include(s => s.Type).FirstOrDefaultAsync(s => s.Id == id, cancellationToken: ct);
		if (entity is null)
			return null;
		var entry = Database.Entry(entity);
		var containerId = entry.Property<Guid?>("ShiftContainerId").CurrentValue;
		var employee = await _keycloakService.GetUserById(entity.EmployeeKeycloakId);
		
		return new()
		{
			ContainerId = containerId ?? Guid.Empty,
			Employee = employee,
			End = entity.End,
			Id = entity.Id,
			Start = entity.Start,
			Type = entity.Type.Adapt<GetShiftTypesResponse>()
		};
	}
}