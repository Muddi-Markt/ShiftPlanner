using FastEndpoints;
using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints;

public abstract class CrudUpdateEndpoint<TRequest> : CrudEndpoint<TRequest, EmptyResponse>
	where TRequest : notnull, new()
{
	public CrudUpdateEndpoint(ShiftPlannerContext database) : base(database)
	{
	}
	
	public abstract Task CrudExecuteAsync(TRequest request, CancellationToken ct);
	public sealed override async Task HandleAsync(TRequest request, CancellationToken ct)
	{
		await CrudExecuteAsync(request, ct);
		//Wow this is a bummer: As refit currently can't handle 204 correctly
		//we have to return 200 and an empty response...
		//see https://github.com/reactiveui/refit/issues/1128
		// if (val.Count == 0)
		// {
		// 	await SendAsync(val, StatusCodes.Status204NoContent, ct);
		// }
	}
	
}