using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Api.Services;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Server.Database.Entities;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Locations.Shifts;

public class GetAllEndpoint : CrudGetAllEndpoint<DefaultGetRequest, GetShiftResponse>
{
	private readonly IKeycloakService _keycloakService;

	public GetAllEndpoint(ShiftPlannerContext database, IKeycloakService keycloakService) : base(database)
	{
		_keycloakService = keycloakService;
	}

	protected override void CrudConfigure()
	{
		Get("/locations/{Id:guid}/shifts");
	}

	public override async Task<List<GetShiftResponse>> CrudExecuteAsync(DefaultGetRequest req, CancellationToken ct)
	{
		var location = await Database.ShiftLocations
			.Include(l => l.Containers)
			.ThenInclude(c => c.Shifts)
			.ThenInclude(s => s.Type)
			.FirstAsync(l => l.Id == req.Id, cancellationToken: ct);

		//TODO populate GetEmployeeResponse in GetShiftResponse with KeycloakService

		List<GetShiftResponse> ret = new();
		foreach (var container in location.Containers)
		{
			foreach (var shift in container.Shifts)
			{
				var response = await MapToShiftResponse(shift);
				ret.Add(response);
			}
		}

		return ret;
	}

	private async Task<GetShiftResponse> MapToShiftResponse(Shift shift)
	{
		var shiftResponse = shift.Adapt<GetShiftResponse>();

		shiftResponse.Employee = await _keycloakService.GetUserById(shift.EmployeeKeycloakId);
		
		return shiftResponse;

	}
}