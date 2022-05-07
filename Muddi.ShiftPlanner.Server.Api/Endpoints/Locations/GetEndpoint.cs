﻿using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Locations;

public class GetEndpoint : CrudGetEndpoint<GetLocationResponse>
{
	public GetEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Get("/locations/{Id:guid}");
	}

	public override async Task<GetLocationResponse?> MuddiExecuteAsync(Guid id, CancellationToken ct) =>
		(await Database.ShiftLocations
			.Include(t => t.Type)
			.Include(t => t.Containers)
			.ThenInclude(c => c.Framework)
			.ThenInclude(f => f.ShiftTypeCounts)
			.FirstOrDefaultAsync(l => l.Id == id, cancellationToken: ct))
		?.Adapt<GetLocationResponse>();
}