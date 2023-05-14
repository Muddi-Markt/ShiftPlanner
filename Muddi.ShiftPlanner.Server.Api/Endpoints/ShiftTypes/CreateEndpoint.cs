using Mapster;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Server.Database.Entities;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.ShiftTypes;

public class CreateEndpoint : CrudCreateEndpoint<CreateShiftTypeRequest, GetShiftTypesResponse, GetEndpoint>
{
	public CreateEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Post("/shift-types");
	}

	public override async Task<GetShiftTypesResponse?> CrudExecuteAsync(CreateShiftTypeRequest req, CancellationToken ct)
	{
		var season = await Database.Seasons.FindAsync(req.SeasonId);
		if (season is null)
		{
			await SendNotFoundAsync(nameof(req.SeasonId));
			return null;
		}

		var type = new ShiftTypeEntity
		{
			Id = Guid.NewGuid(),
			Name = req.Name,
			Color = req.Color,
			Season = season,
			OnlyAssignableByAdmin = req.OnlyAssignableByAdmin,
			StartingTimeShift = req.StartingTimeShift,
			Description = string.IsNullOrWhiteSpace(req.Description) ? null : req.Description
		};
		Database.Add(type);
		await Database.SaveChangesAsync(ct);
		return type.Adapt<GetShiftTypesResponse>();
	}
}