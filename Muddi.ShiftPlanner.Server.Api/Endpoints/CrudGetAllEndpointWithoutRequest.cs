using FastEndpoints;
using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints;

public abstract class CrudGetAllEndpointWithoutRequest<TResponse> : CrudGetAllEndpoint<EmptyRequest, TResponse>
	where TResponse : notnull, new()
{
	protected CrudGetAllEndpointWithoutRequest(ShiftPlannerContext database) : base(database)
	{
	}
}

public abstract class CrudGetAllEndpoint<TRequest, TResponse> : CrudEndpoint<TRequest, List<TResponse>>
	where TRequest : notnull, new()
	where TResponse : notnull, new()
{
	public abstract Task<List<TResponse>> CrudExecuteAsync(TRequest request, CancellationToken ct);

	public sealed override void Configure()
	{
		Options(t =>
		{
			t.Produces<DefaultEnumerableResponse<TResponse>>();
			t.Produces(StatusCodes.Status204NoContent);
		});
		base.Configure();
	}

	public sealed override async Task HandleAsync(TRequest request, CancellationToken ct)
	{
		var val = await CrudExecuteAsync(request, ct);
		Response = new List<TResponse>(val);
		if (val.Count == 0)
		{
			await SendNoContentAsync(ct);
		}
	}

	protected CrudGetAllEndpoint(ShiftPlannerContext database) : base(database)
	{
	}
}