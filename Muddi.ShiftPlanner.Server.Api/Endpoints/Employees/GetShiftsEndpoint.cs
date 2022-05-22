using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Api.Extensions;
using Muddi.ShiftPlanner.Server.Api.Services;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Server.Database.Entities;
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

	public override async Task<List<GetShiftResponse>> CrudExecuteAsync(GetShiftsFromEmployeeRequest request, CancellationToken ct)
	{
		if (request.Count == 0)
			return new();
		var user = await _keycloakService.GetUserById(request.Id);
		IQueryable<ShiftEntity> b = Database.Shifts
			.Include(s => s.Type)
			.Include(s => s.ShiftContainer)
			.ThenInclude(c => c.Location)
			.Where(s => s.EmployeeKeycloakId == request.Id)
			.OrderBy(s => s.Start);
		if (request.Count > 0)
			b = b.Take(request.Count);
		
		return await 
			b.Select(s => s.MapToShiftResponse(user))
				.ToListAsync(cancellationToken: ct);
	}
}


public class GetShiftsFromEmployeeRequest{
	/// <summary>
	/// EmployeeID
	/// </summary>
	public Guid Id { get; set; }

	/// <summary>
	/// Most recent count, if smaller then 0 all will be shown
	/// </summary>
	public int Count { get; set; } = -1;
}