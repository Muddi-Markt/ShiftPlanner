using FastEndpoints;
using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints;

public abstract class CrudEndpointWithoutRequest<TResponse> : CrudEndpoint<EmptyRequest, TResponse> where TResponse : notnull
{
	protected CrudEndpointWithoutRequest(ShiftPlannerContext database) : base(database)
	{
	}
}