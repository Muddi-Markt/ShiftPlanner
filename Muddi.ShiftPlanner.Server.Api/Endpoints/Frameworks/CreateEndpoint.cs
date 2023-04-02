using Mapster;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Server.Database.Entities;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Frameworks;

public class CreateEndpoint : CrudCreateEndpoint<CreateFrameworkRequest, GetFrameworkResponse, GetEndpoint>
{
	public CreateEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Post("/frameworks");
	}

	public override async Task<GetFrameworkResponse?> CrudExecuteAsync(CreateFrameworkRequest req, CancellationToken ct)
	{
		var season = await Database.Seasons.FindAsync(req.SeasonId);
		if (season is null)
		{
			await SendNotFoundAsync(nameof(req.SeasonId));
			return null;
		}
		var framework = new ShiftFrameworkEntity
		{
			Id = Guid.NewGuid(),
			Name = req.Name,
			Season = season,
			SecondsPerShift = req.SecondsPerShift,
			ShiftTypeCounts = new List<ShiftFrameworkTypeCountEntity>()
		};

		foreach (var countDto in req.TypeCounts)
		{
			var shiftType = new ShiftTypeEntity() { Id = countDto.ShiftTypeId };
			Database.Attach(shiftType);
			var countEntity = new ShiftFrameworkTypeCountEntity()
			{
				Id = Guid.NewGuid(),
				Count = countDto.Count,
				ShiftFramework = framework,
				ShiftType = shiftType
			};
			framework.ShiftTypeCounts.Add(countEntity);
		}

		Database.ShiftFrameworks.Add(framework);
		await Database.SaveChangesAsync(ct);
		return framework.Adapt<GetFrameworkResponse>();
	}
}