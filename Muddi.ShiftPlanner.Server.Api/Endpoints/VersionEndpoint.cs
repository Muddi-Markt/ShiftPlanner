using System.Reflection;
using FastEndpoints;
using Muddi.ShiftPlanner.Shared.Contracts.v1;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints;

public sealed class VersionEndpoint : EndpointWithoutRequest<object>
{
	private static readonly Version? AssemblyVersion =
		Assembly.GetEntryAssembly()?.GetName().Version;

	public override void Configure()
	{
		Get("/version");
		Throttle(30, 60);
		AllowAnonymous();
	}

	public override Task<object> ExecuteAsync(CancellationToken ct)
	{
		return Task.FromResult<object>(new { version = AssemblyVersion });
	}
}