using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Api.Extensions;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Server.Database.Entities;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Containers;

public class CreateEndpoint : CrudCreateEndpoint<CreateContainerRequest, GetContainerResponse, GetEndpoint>
{
	public CreateEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Post("/containers");
		Options(t => t.Produces(StatusCodes.Status404NotFound));
	}

	public override async Task<GetContainerResponse?> CrudExecuteAsync(CreateContainerRequest req, CancellationToken ct)
	{
		var framework = await Database.ShiftFrameworks
			.Include(t => t.ShiftTypeCounts)
			.ThenInclude(st => st.ShiftType)
			.FirstOrDefaultAsync(t => t.Id == req.FrameworkId, cancellationToken: ct);
		if (framework is null)
		{
			await SendNotFoundAsync(nameof(req.FrameworkId));
			return null;
		}
		
		var location = await Database.ShiftLocations
			.CheckAdminOnly(User)
			.Include(t => t.Containers)
			.FirstOrDefaultAsync(t => t.Id == req.LocationId, cancellationToken: ct);
		if (location is null)
		{
			await SendNotFoundAsync(nameof(req.LocationId));
			return null;
		}

		if (location.Containers.Any(c => c.Start == req.Start))
		{
			await SendConflictAsync("A container with the same starting time already exist");
			return null;
		}

		var container = new ShiftContainerEntity
		{
			Id = Guid.NewGuid(),
			Start = req.Start,
			End = req.Start + framework.TimePerShift * req.TotalShifts,
			TotalShifts = req.TotalShifts,
			Framework = framework,
			Color = req.Color
		};

		
		Database.Containers.Add(container);
		location.Containers.Add(container);
		await Database.SaveChangesAsync(ct);
		return container.Adapt<GetContainerResponse>();
	}
}