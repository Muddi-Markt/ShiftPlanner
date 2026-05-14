using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Shared.Entities;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Settings;

/// <summary>
/// Updates app-wide settings (StartTime, EndTime).
/// Admin-only endpoint. Creates the settings row if it doesn't exist yet.
/// </summary>
public class UpdateEndpoint : CrudEndpoint<UpdateAppSettingsRequest, EmptyResponse>
{
	public UpdateEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Put("/settings");
	}

	public override async Task<EmptyResponse> ExecuteAsync(UpdateAppSettingsRequest req, CancellationToken ct)
	{
		var entity = await Database.AppSettings.FirstOrDefaultAsync(ct);

		var settings = new ApplicationSettings
		{
			StartTime = req.StartTime,
			EndTime = req.EndTime
		};

		if (entity is null)
		{
			// First time — insert the row
			entity = new() { Settings = settings };
			Database.AppSettings.Add(entity);
		}
		else
		{
			entity.Settings = settings;
		}

		await Database.SaveChangesAsync(ct);
		return EmptyResponse.Instance;
	}
}