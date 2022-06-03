﻿using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Shared.Contracts.v1;
using Namotion.Reflection;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Locations.Shifts;

public class GetAllShiftsCountEndpoint : CrudGetAllEndpointWithoutRequest<GetShiftsCountResponse>
{
	public GetAllShiftsCountEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Roles(ApiRoles.Editor, ApiRoles.Viewer);
		Get("/locations/get-all-shifts-count");
	}

	public override async Task<List<GetShiftsCountResponse>?> CrudExecuteAsync(EmptyRequest request, CancellationToken ct)
	{
		var total = await Database.ShiftLocations
			.GroupBy(s => s.Id)
			.Select(l =>
				new GetShiftsCountResponse
				{
					Id = l.Key,
					TotalShifts = l.Sum(l2 => l2.Containers.Sum(c => c.TotalShifts * c.Framework.ShiftTypeCounts
								.Sum(st => st.Count))),
					AssignedShifts = l.Sum(q => q.Containers.Sum(c => c.Shifts.Count(s => s.ShiftContainer.Location.Id == l.Key)))
				}).ToListAsync(cancellationToken: ct);
		return total;
	}
}