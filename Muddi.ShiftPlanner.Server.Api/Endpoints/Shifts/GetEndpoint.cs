using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Api.Extensions;
using Muddi.ShiftPlanner.Server.Api.Services;
using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Shifts;

public class GetEndpoint : CrudGetEndpoint<GetShiftResponse>
{
	private readonly IKeycloakService _keycloakService;

	public GetEndpoint(ShiftPlannerContext database, IKeycloakService keycloakService) : base(database)
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
		var entity = await Database.Shifts
			.Include(s => s.Type)
			.Include(s => s.ShiftContainer)
			.ThenInclude(c => c.Location)
			.FirstOrDefaultAsync(s => s.Id == id, cancellationToken: ct);
		if (entity is null)
			return null;
		var employee = await _keycloakService.GetUserByIdAsync(entity.EmployeeKeycloakId);

		return entity.MapToShiftResponse(employee.MapToEmployeeResponse(User));
	}
}