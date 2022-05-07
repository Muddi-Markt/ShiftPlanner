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
		var framework = new ShiftFramework
		{
			Id = Guid.NewGuid(),
			Name = req.Name,
			SecondsPerShift = req.SecondsPerShift,
			ShiftTypeCounts = new List<ShiftFrameworkTypeCount>()
		};

		foreach (var countDto in req.TypeCounts)
		{
			var shiftType = new ShiftType() { Id = countDto.ShiftTypeId };
			Database.Attach(shiftType);
			var countEntity = new ShiftFrameworkTypeCount()
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