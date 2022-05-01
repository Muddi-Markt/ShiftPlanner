using FastEndpoints;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints;

[HttpGet("/")]
public class TestEndpoint : EndpointWithoutRequest
{
	public override Task HandleAsync(CancellationToken ct)
	{
		return SendAsync($"Hello {User.Identity?.Name}");
	}
}