﻿using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Server.Database.Entities;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Locations;

public class CreateEndpoint : CrudCreateEndpoint<CreateLocationRequest, GetLocationResponse, GetEndpoint>
{
	public CreateEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Post("/locations");
		Options(t => t.Produces(StatusCodes.Status404NotFound));
	}

	public override async Task<GetLocationResponse?> CrudExecuteAsync(CreateLocationRequest req, CancellationToken ct)
	{
		var type = await Database.ShiftLocationTypes.FirstOrDefaultAsync(t => t.Id == req.TypeId, ct);
		if (type is null)
		{
			await SendNotFoundAsync(nameof(req.TypeId));
			return null;
		}

		var location = new ShiftLocationEntity
		{
			Id = Guid.NewGuid(),
			Name = req.Name,
			Type = type,
			Containers = new List<ShiftContainerEntity>()
		};
		Database.Add(location);
		await Database.SaveChangesAsync(ct);
		return location.Adapt<GetLocationResponse>();
	}
}