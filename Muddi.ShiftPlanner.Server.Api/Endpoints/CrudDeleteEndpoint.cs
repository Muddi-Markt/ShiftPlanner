using FastEndpoints;
using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints;

public abstract class CrudDeleteEndpoint : CrudEndpoint<DefaultGetRequest, EmptyResponse>
{
	protected abstract Task<DeleteResponse> CrudExecuteAsync(Guid id, CancellationToken ct);

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
		var res = await CrudExecuteAsync(req.Id, ct);
		switch (res)
		{
			case DeleteResponse.NotFound:
				await SendNotFoundAsync(ct);
				return;
			case DeleteResponse.OK:
				await SendNoContentAsync(ct);
				return;
			//On other only return as the CrudExecuteAsync method already sent an response
			case DeleteResponse.Other:
			default:
				return;
		}
	}

	protected CrudDeleteEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected enum DeleteResponse
	{
		
		OK,
		NotFound,
		/// <summary>
		/// When this is used you have to call SendAsync or similar
		/// </summary>
		Other
	}
}