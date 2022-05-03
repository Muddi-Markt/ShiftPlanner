using FastEndpoints;
using Muddi.ShiftPlanner.Server.Api.Contracts.Requests;
using Muddi.ShiftPlanner.Server.Api.Contracts.Responses;
using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints;

public abstract class CrudGetAllEndpoint<TResponse> : CrudEndpointWithoutRequest<DefaultEnumerableResponse<TResponse>>
	where TResponse : notnull, new()
{
	public abstract Task<ICollection<TResponse>> CrudExecuteAsync(CancellationToken ct);

	public sealed override void Configure()
	{
		Options(t =>
		{
			t.Produces<DefaultEnumerableResponse<TResponse>>();
			t.Produces(StatusCodes.Status204NoContent);
		});
		base.Configure();
	}

	public sealed override async Task HandleAsync(EmptyRequest _, CancellationToken ct)
	{
		var val = await CrudExecuteAsync(ct);
		Response = new DefaultEnumerableResponse<TResponse>() { Data = val };
		if (val.Count == 0)
		{
			await SendNoContentAsync(ct);
		}
	}

	protected CrudGetAllEndpoint(ShiftPlannerContext database) : base(database)
	{
	}
}