using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Server.Database.Entities;
using Muddi.ShiftPlanner.Shared.Contracts.v1;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Locations
{
	public class UpdateEndpoint : CrudUpdateEndpoint<UpdateLocationRequest>
	{
		public UpdateEndpoint(ShiftPlannerContext database) : base(database)
		{
		}

		protected override void CrudConfigure()
		{
			Put("/locations/{Id}");
		}

		public override async Task CrudExecuteAsync(UpdateLocationRequest request, CancellationToken ct)
		{
			var entity = await Database.ShiftLocations
				.Include(l => l.Season)
				.Include(f => f.Type)
				.FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken: ct);
			if (entity is null)
			{
				await Send.NotFoundAsync("location", ct);
				return;
			}

			entity.Name = request.Name;
			if (entity.Type.Id != request.TypeId)
			{
				entity.Type = new() { Id = request.TypeId };
				Database.Attach(entity.Type);
			}

			await Database.SaveChangesAsync(ct);
		}
	}
}