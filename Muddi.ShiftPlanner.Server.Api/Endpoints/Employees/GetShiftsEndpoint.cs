using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Api.Extensions;
using Muddi.ShiftPlanner.Server.Api.Services;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Server.Database.Entities;
using Muddi.ShiftPlanner.Shared;
using Muddi.ShiftPlanner.Shared.Contracts.v1;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Employees;

public class GetShiftsEndpoint : CrudGetAllEndpoint<GetShiftsFromEmployeeRequest, GetShiftResponse>
{
	private readonly IKeycloakService _keycloakService;

	public GetShiftsEndpoint(ShiftPlannerContext database, IKeycloakService keycloakService) : base(database)
	{
		_keycloakService = keycloakService;
	}

	protected override void CrudConfigure()
	{
		Roles(ApiRoles.Editor, ApiRoles.Viewer);
		Get("/employees/{Id}/shifts");
	}

	public override async Task<List<GetShiftResponse>?> CrudExecuteAsync(GetShiftsFromEmployeeRequest request, CancellationToken ct)
	{
		var count = request.Count ?? -1;
		var date = request.StartingFrom?.ToUniversalTime();
		var id = request.Id ?? User.GetKeycloakId();
		if (count == 0)
			return new();
		bool isRequestUserOrAdmin = id == User.GetKeycloakId() || User.IsInRole(ApiRoles.Admin);
		if (!isRequestUserOrAdmin)
		{
			await Send.ForbiddenAsync(ct);
			return null;
		}

		var user = await _keycloakService.GetUserByIdAsync(id);
		IQueryable<ShiftEntity> b = Database.Shifts
			.Where(s => s.Type.Season.Id == request.SeasonId)
			.Include(s => s.Type)
			.Include(s => s.ShiftContainer)
			.ThenInclude(c => c.Location)
			.Where(s => s.EmployeeKeycloakId == id)
			.OrderBy(s => s.Start);
		if (date is not null)
			b = b.Where(s => s.Start >= date);
		if (count > 0)
			b = b.Take(count);

		return await
			b.Select(s => s.MapToShiftResponse(user))
				.ToListAsync(cancellationToken: ct);
	}
}