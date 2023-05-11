using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.ShiftTypes;

public class UpdateEndpoint : CrudUpdateEndpoint<UpdateShiftTypeRequest>
{
	public UpdateEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Put("/shift-types/{Id}");
	}

	public override async Task CrudExecuteAsync(UpdateShiftTypeRequest request, CancellationToken ct)
	{
		var entity = await Database.ShiftTypes.Include(x => x.Season)
			.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: ct);
		if (entity is null)
		{
			await SendNotFoundAsync("shift-type");
			return;
		}

		entity.Color = request.Color;
		entity.Name = request.Name;
		entity.StartingTimeShift = request.StartingTimeShift;
		entity.OnlyAssignableByAdmin = request.OnlyAssignableByAdmin;
		await Database.SaveChangesAsync(ct);
	}
}