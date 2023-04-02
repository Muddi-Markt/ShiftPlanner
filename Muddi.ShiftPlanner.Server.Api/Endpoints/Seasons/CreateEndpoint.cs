using Mapster;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Server.Database.Entities;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses.Seasons;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Seasons;

public class CreateEndpoint : CrudCreateEndpoint<CreateSeasonRequest, GetSeasonResponse, GetEndpoint>
{
	public CreateEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Post("/seasons");
	}

	public override async Task<GetSeasonResponse?> CrudExecuteAsync(CreateSeasonRequest req, CancellationToken ct)
	{
		var season = new SeasonEntity
		{
			Name = req.Name,
			StartDate = req.StartDate,
			EndDate = req.EndDate
		};


		Database.Seasons.Add(season);
		await Database.SaveChangesAsync(ct);
		return season.Adapt<GetSeasonResponse>();
	}
}