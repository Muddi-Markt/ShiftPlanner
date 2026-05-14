using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Settings;

/// <summary>
/// Returns app-wide settings (StartTime, EndTime) stored in the database.
/// If no settings exist in the DB, returns defaults.
/// </summary>
public class GetEndpoint : EndpointWithoutRequest<GetAppSettingsResponse>
{
	public GetEndpoint(ShiftPlannerContext database)
	{
		Database = database;
	}

	private ShiftPlannerContext Database { get; }

	public override void Configure()
	{
		Get("/settings");
	}

	public override async Task<GetAppSettingsResponse> ExecuteAsync(CancellationToken ct)
	{
		var entity = await Database.AppSettings.FirstOrDefaultAsync(ct);
		return new GetAppSettingsResponse
		{
			Settings = entity?.Settings ?? new()
		};
	}
}
