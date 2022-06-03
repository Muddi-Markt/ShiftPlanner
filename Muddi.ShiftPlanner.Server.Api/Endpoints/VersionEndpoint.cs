using System.Reflection;
using FastEndpoints;
using Muddi.ShiftPlanner.Shared.Contracts.v1;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints;

public sealed class VersionEndpoint : EndpointWithoutRequest<object>
{
	private static readonly Version? AssemblyVersion =
		Assembly.GetEntryAssembly()?.GetName().Version;

	private static readonly DateTime CompilationDate = AssemblyVersion is null
		? new DateTime(2000, 1, 1)
		: new DateTime(2000, 1, 1).AddDays(AssemblyVersion.Build).AddSeconds(AssemblyVersion.Revision * 2).ToUniversalTime();

	public override void Configure()
	{
		Get("/version");
		Throttle(30, 60);
		AllowAnonymous();
	}

	public override Task<object> ExecuteAsync(CancellationToken ct)
	{
		return Task.FromResult<object>(new { AssemblyVersion, CompilationDate });
	}
}