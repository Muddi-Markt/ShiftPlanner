﻿using Mapster;
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
			SecondsPerShift = req.SecondsPerShift,
			ShiftTypeCounts = new List<ShiftFrameworkTypeCount>()
		};
		framework.ShiftTypeCounts.AddRange(req.TypeCounts.Select(t => new ShiftFrameworkTypeCount()
		{
			Id = Guid.NewGuid(),
			Count = t.Count,
			ShiftFrameworkId = framework.Id,
			ShiftTypeId = t.ShiftTypeId
		}));

		Database.ShiftFrameworks.Add(framework);
		await Database.SaveChangesAsync(ct);
		return framework.Adapt<GetFrameworkResponse>();
	}
}