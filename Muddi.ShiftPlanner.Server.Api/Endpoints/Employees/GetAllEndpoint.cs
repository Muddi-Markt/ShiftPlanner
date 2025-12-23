using FastEndpoints;
using Muddi.ShiftPlanner.Server.Api.Extensions;
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
		// Roles(ApiRoles.Admin); default role is admin, so we dont have to set it here
		Get("/employees");
	}

	public override async Task<List<GetEmployeeResponse>?> CrudExecuteAsync(EmptyRequest _, CancellationToken ct)
	{
		var getEmployeeResponses = await _keycloakService.GetUsers();
		var isAdmin = User.IsInRole(ApiRoles.Admin);
		return getEmployeeResponses
			.OrderBy(x => x.FirstName, StringComparer.InvariantCultureIgnoreCase)
			.Select(x => x.MapToEmployeeResponse(isAdmin))
			.ToList();
	}
}