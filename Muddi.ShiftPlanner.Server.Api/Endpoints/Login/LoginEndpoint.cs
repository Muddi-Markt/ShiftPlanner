using FastEndpoints;
using FluentValidation.Results;
using Muddi.ShiftPlanner.Server.Api.Services;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Login;

public class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
	private readonly IKeycloakService _keycloakService;

	public LoginEndpoint(IKeycloakService keycloakService)
	{
		_keycloakService = keycloakService;
	}

	public override void Configure()
	{
		Post("/login");
		AllowAnonymous();
	}


	public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
	{
		var apiResp = await _keycloakService.GetToken(new(req.Email, req.Password, "shift-planner"));
		if (!apiResp.IsSuccessStatusCode || apiResp.Content is null)
		{
			ValidationFailures.Add(new ValidationFailure("Api", apiResp.Error?.Content ?? apiResp.ReasonPhrase));
			await SendErrorsAsync((int)apiResp.StatusCode, ct);
			return;
		}

		Response = new LoginResponse
		{
			AccessToken = apiResp.Content.AccessToken,
			ExpiresAt = DateTime.UtcNow.AddSeconds(apiResp.Content.ExpiresIn)
		};
	}
}