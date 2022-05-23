using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints;

public abstract class CrudGetAllEndpoint<TRequest, TResponse> : CrudEndpoint<TRequest, List<TResponse>>
	where TRequest : notnull, new()
	where TResponse : notnull, new()
{
	public abstract Task<List<TResponse>?> CrudExecuteAsync(TRequest request, CancellationToken ct);

	public sealed override void Configure()
	{
		Options(t =>
		{
			t.Produces<DefaultEnumerableResponse<TResponse>>();
			// t.Produces(StatusCodes.Status204NoContent);
		});
		base.Configure();
	}

	public sealed override async Task HandleAsync(TRequest request, CancellationToken ct)
	{
		var val = await CrudExecuteAsync(request, ct);
		if (val is not null)
			Response = val;

		//Wow this is a bummer: As refit currently can't handle 204 correctly
		//we have to return 200 and an empty response...
		//see https://github.com/reactiveui/refit/issues/1128
		// if (val.Count == 0)
		// {
		// 	await SendAsync(val, StatusCodes.Status204NoContent, ct);
		// }
	}

	protected CrudGetAllEndpoint(ShiftPlannerContext database) : base(database)
	{
	}
}