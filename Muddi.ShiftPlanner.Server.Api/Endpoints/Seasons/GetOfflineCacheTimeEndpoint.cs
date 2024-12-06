using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Seasons;

public class GetOfflineCacheTimeEndpoint : CrudGetEndpoint<SeasonOfflineCacheTimeResponse>
{
	public GetOfflineCacheTimeEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Get("/seasons/{id:guid}/cache-time");
	}

	public override Task<SeasonOfflineCacheTimeResponse?> CrudExecuteAsync(Guid id, CancellationToken ct)
	{
		return Task.FromResult(new SeasonOfflineCacheTimeResponse { LastCacheTime = Database.GetLastCacheTime() })!;
	}
}

public class SeasonOfflineCacheTimeResponse
{
	public DateTime LastCacheTime { get; init; }
}