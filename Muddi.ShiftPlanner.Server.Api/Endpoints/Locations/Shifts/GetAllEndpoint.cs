using System.Diagnostics;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Api.Extensions;
using Muddi.ShiftPlanner.Server.Api.Services;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Server.Database.Entities;
using Muddi.ShiftPlanner.Shared.Contracts.v1;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Locations.Shifts;

public class GetAllEndpoint : CrudGetAllEndpoint<GetAllShiftsForLocationRequest, GetShiftResponse>
{
	private readonly IKeycloakService _keycloakService;

	public GetAllEndpoint(ShiftPlannerContext database, IKeycloakService keycloakService) : base(database)
	{
		_keycloakService = keycloakService;
	}

	protected override void CrudConfigure()
	{
		Roles(ApiRoles.Editor, ApiRoles.Viewer);
		Get("/locations/{Id:guid}/shifts");
	}

	public override async Task<List<GetShiftResponse>?> CrudExecuteAsync(GetAllShiftsForLocationRequest req,
		CancellationToken ct)
	{
		//TODO why is DateTime always converted to local time by fast-endpoints?!
		var start = req.Start.ToUniversalTime();
		var end = req.End.ToUniversalTime();
		var location = await Database.ShiftLocations
			.Include(l => l.Containers)
			.ThenInclude(c => c.Shifts.Where(s =>
				// ReSharper disable once SimplifyConditionalTernaryExpression
				(req.KeycloakEmployeeId.HasValue ? s.EmployeeKeycloakId == req.KeycloakEmployeeId.Value : true) &&
				(s.Start >= start && s.Start < end)))
			.ThenInclude(s => s.Type)
			.AsSingleQuery()
			.FirstAsync(l => l.Id == req.Id!.Value, cancellationToken: ct);
		List<GetShiftResponse> ret = new();
		foreach (var shift in location.Containers
			         .SelectMany(c => c.Shifts)
			         .OrderBy(s => s.Start)
			         .ThenBy(s => s.EmployeeKeycloakId))
		{
			var response = await MapToShiftResponse(shift);
			ret.Add(response);
		}

		return ret;
	}

	private async Task<GetShiftResponse> MapToShiftResponse(ShiftEntity shift)
	{
		var user = await _keycloakService.GetUserByIdAsync(shift.EmployeeKeycloakId);
		return shift.MapToShiftResponse(user);
	}
}