using FastEndpoints;
using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints;

public abstract class CrudDeleteEndpoint : CrudEndpoint<DefaultGetRequest, EmptyResponse>
{
	public abstract Task<bool> CrudExecuteAsync(Guid id, CancellationToken ct);

	public override void Configure()
	{
		Options(t =>
		{
			t.Produces(StatusCodes.Status204NoContent);
			t.Produces(StatusCodes.Status404NotFound);
		});
		base.Configure();
	}

	public sealed override async Task HandleAsync(DefaultGetRequest req, CancellationToken ct)
	{
		var success = await CrudExecuteAsync(req.Id, ct);
		if (!success)
		{
			await SendNotFoundAsync(ct);
			return;
		}

		await SendNoContentAsync(ct);
	}

	protected CrudDeleteEndpoint(ShiftPlannerContext database) : base(database)
	{
	}
}