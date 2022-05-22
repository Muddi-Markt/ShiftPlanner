using FastEndpoints;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Api.Services;
using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Employees;

public class GetAllEndpoint : CrudGetAllEndpointWithoutRequest<GetEmployeeResponse>
{
	private readonly IKeycloakService _keycloakService;

	public GetAllEndpoint(ShiftPlannerContext database, IKeycloakService keycloakService) : base(database)
	{
		_keycloakService = keycloakService;
	}

	protected override void CrudConfigure()
	{
		Get("/employees");
	}

	public override async Task<List<GetEmployeeResponse>> CrudExecuteAsync(EmptyRequest _, CancellationToken ct)
	{
		return (await _keycloakService.GetUsers()).ToList();
	}
}