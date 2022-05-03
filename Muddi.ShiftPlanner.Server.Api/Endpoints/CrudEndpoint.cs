using FastEndpoints;
using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints;

public abstract class CrudEndpoint<TRequest, TResponse> : Endpoint<TRequest, TResponse>
	where TRequest : notnull, new()
	where TResponse : notnull, new()
{
	protected CrudEndpoint(ShiftPlannerContext database)
	{
		Database = database;
	}

	protected ShiftPlannerContext Database { get; }
	protected abstract void CrudConfigure();

	public override void Configure()
	{
		CrudConfigure();
#if DEBUG
		AllowAnonymous();
#endif
		Options(t => t.Produces<TResponse>());
		Throttle(60, 60);
	}

	protected Task SendNotFoundAsync(string idName)
	{
		return SendStringAsync($"{idName} does not exist", StatusCodes.Status404NotFound);
	}
}