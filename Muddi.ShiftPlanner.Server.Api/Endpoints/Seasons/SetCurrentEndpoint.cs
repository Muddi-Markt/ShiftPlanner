using System.Net;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Seasons;

public class SetCurrentEndpoint : CrudEndpoint<DefaultGetRequest, EmptyResponse>
{
	public SetCurrentEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Put("/seasons/current/{Id:guid}");
	}

	public override async Task<EmptyResponse> ExecuteAsync(DefaultGetRequest req, CancellationToken ct)
	{
		var selected = await Database.Seasons.Where(s => s.IsSelectedAsCurrent).ToListAsync(cancellationToken: ct);
		bool alreadySelected = false;
		foreach (var seasonEntity in selected)
		{
			if (seasonEntity.Id == req.Id)
			{
				Logger.LogInformation("The season {SeasonsId} is already selected", req.Id);
				alreadySelected = true;
				continue;
			}

			seasonEntity.IsSelectedAsCurrent = false;
		}

		if (!alreadySelected)
		{
			Logger.LogInformation("Select {SeasonsId} as current Season", req.Id);
			var entity = await Database.Seasons.FirstAsync(x => x.Id == req.Id, cancellationToken: ct);
			entity.IsSelectedAsCurrent = true;
		}

		await Database.SaveChangesAsync(ct);
		return new EmptyResponse();
	}
}

public class SetCurrentSeasonRequest
{
}