using FastEndpoints;

namespace Muddi.ShiftPlaner.Server.Api.Endpoints;

[HttpGet("/")]
public class TestEndpoint : EndpointWithoutRequest
{
	public override Task HandleAsync(CancellationToken ct)
	{
		return SendAsync($"Hello {User.Identity?.Name}");
	}
}