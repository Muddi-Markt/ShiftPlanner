using FastEndpoints;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints;

public class AdminAsUserEndpoint : EndpointWithoutRequest
{
	public override void Configure()
	{
		Get("/admin/as-user");
	}

	public override Task HandleAsync(CancellationToken ct)
	{
		var claims = User.Claims.ToLookup(x => x.Type, x => x.Value);
		object res = claims.Contains("roles")
			? new { roles = claims["roles"] }
			: claims;
		return Send.OkAsync(res, cancellation: ct);
	}
}