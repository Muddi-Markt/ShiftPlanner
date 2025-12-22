using FastEndpoints;
using Muddi.ShiftPlanner.Shared.Contracts.v1;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints;

public class AdminAsAdminEndpoint : EndpointWithoutRequest
{
	public override void Configure()
	{
		Get("/admin/as-admin");
		Roles(ApiRoles.Admin);
	}

	public override Task HandleAsync(CancellationToken ct)
	{
		return Send.OkAsync("Hi admin!", cancellation: ct);
	}
}