using FastEndpoints;

namespace Muddi.ShiftPlaner.Server.Api.Endpoints;

public class AdminEndpoint : EndpointWithoutRequest
{
	public override void Configure()
	{
		Get("/admin");
		Roles("admin");
	}

	public override Task HandleAsync(CancellationToken ct)
	{
		return SendAsync("Hi admin!", cancellation: ct);
	}
}