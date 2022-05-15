using FastEndpoints;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints;

public class AdminEndpoint : EndpointWithoutRequest
{
	public override void Configure()
	{
		Get("/admin");
		Roles(ApiRoles.Admin);
	}

	public override Task HandleAsync(CancellationToken ct)
	{
		return SendAsync("Hi admin!", cancellation: ct);
	}
}