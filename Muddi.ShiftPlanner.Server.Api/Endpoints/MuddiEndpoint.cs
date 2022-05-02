using FastEndpoints;
using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints;

public abstract class MuddiEndpointWithoutRequest<TResponse> : MuddiEndpoint<EmptyRequest, TResponse> where TResponse : notnull, new()
{
	protected MuddiEndpointWithoutRequest(ShiftPlannerContext database) : base(database)
	{
	}
}

public abstract class MuddiEndpoint<TRequest, TResponse> : Endpoint<TRequest, TResponse>
	where TRequest : notnull, new()
	where TResponse : notnull, new()
{
	protected MuddiEndpoint(ShiftPlannerContext database)
	{
		Database = database;
	}

	protected ShiftPlannerContext Database { get; }
	protected abstract void MuddiConfigure();

	public sealed override void Configure()
	{
		MuddiConfigure();
#if DEBUG
		AllowAnonymous();
#endif

		Throttle(60, 60);
	}

	protected Task SendNotFoundAsync(string idName)
	{
		return SendStringAsync($"{idName} does not exist", StatusCodes.Status404NotFound);
	}
}