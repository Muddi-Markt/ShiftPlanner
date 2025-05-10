using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Api.Extensions;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Server.Database.Entities;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses.Seasons;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Seasons;

public class GetOfflineDataEndpoint : CrudGetEndpoint<SeasonOfflineDataResponse>
{
	public GetOfflineDataEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Get("/seasons/{id:guid}/offline-data");
	}

	public override async Task<SeasonOfflineDataResponse?> CrudExecuteAsync(Guid id, CancellationToken ct)
	{
		var data = await Database.GetSeasonOfflineDataCache(id, ct);
		return new() { SeasonOfflineData = data };
	}
}

public class SeasonOfflineDataResponse
{
	public SeasonOfflineDataDbto SeasonOfflineData { get; init; }
}