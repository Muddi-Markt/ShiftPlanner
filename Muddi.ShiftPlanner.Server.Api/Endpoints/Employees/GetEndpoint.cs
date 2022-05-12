using FastEndpoints;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Muddi.ShiftPlanner.Server.Api.Services;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Employees;

public class GetEndpoint : Endpoint<DefaultGetRequest, GetEmployeeResponse>
{
	private readonly IKeycloakService _keycloakService;

	public GetEndpoint(IKeycloakService keycloakService)
	{
		_keycloakService = keycloakService;
	}

	public override void Configure()
	{
		Get("/employees/{Id}");
	}


	public override async Task HandleAsync(DefaultGetRequest req, CancellationToken ct)
	{
		//It would also work with an access token, but as we would need to set the client to confidential
		//and we use an wasm frontend, we don't want this (at least for now...)
		//For further infos see: https://stackoverflow.com/questions/66452108/keycloak-get-users-returns-403-forbidden/66454728#66454728
		// var token = await HttpContext.GetTokenAsync(JwtBearerDefaults.AuthenticationScheme, "access_token");
		Response = await _keycloakService.GetUserById(req.Id);
	}
}