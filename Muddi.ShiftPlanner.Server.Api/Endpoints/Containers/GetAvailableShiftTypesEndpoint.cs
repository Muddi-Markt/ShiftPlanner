using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Containers;

public class GetAvailableShiftTypesEndpoint : CrudGetAllEndpoint<GetAvailableShiftTypesRequest, GetShiftTypesResponse>
{
	public GetAvailableShiftTypesEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void CrudConfigure()
	{
		Get("/containers/{ContainerId}/get-available-shift-types");
	}

	public override async Task<List<GetShiftTypesResponse>> CrudExecuteAsync(GetAvailableShiftTypesRequest request, CancellationToken ct)
	{
		var res = new List<GetShiftTypesResponse>();
		var requestStartTime = request.StartTime.ToUniversalTime();
		var container = await Database.Containers
			.Include(c => c.Framework)
			.ThenInclude(f => f.ShiftTypeCounts)
			.ThenInclude(stc => stc.ShiftType)
			.Include(c => c.Shifts)
			.ThenInclude(s => s.Type)
			.FirstOrDefaultAsync(c => c.Id == request.ContainerId, cancellationToken: ct);
		if (container is null)
		{
			await SendNotFoundAsync("container");
			return res;
		}
		
		var counter = container.Framework.ShiftTypeCounts.ToList();
		foreach (var shift in container.Shifts.Where(s => s.Start == requestStartTime))
		{
			
			var q = counter.Single(c => c.ShiftType.Id == shift.Type.Id);
			q.Count--;
		}

		return counter
			.Where(c => c.Count > 0)
			.Select(c => new GetShiftTypesResponse { Id = c.ShiftType.Id, Name = c.ShiftType.Name })
			.ToList();
	}
}

public class GetAvailableShiftTypesRequest
{
	public Guid ContainerId { get; set; }
	[BindFrom("start")]
	public DateTime StartTime { get; set; }
}