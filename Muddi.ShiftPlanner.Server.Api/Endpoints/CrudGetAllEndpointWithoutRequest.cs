using FastEndpoints;
using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints;

public abstract class CrudGetAllEndpointWithoutRequest<TResponse> : CrudGetAllEndpoint<EmptyRequest, TResponse>
	where TResponse : notnull
{
	protected CrudGetAllEndpointWithoutRequest(ShiftPlannerContext database) : base(database)
	{
	}
}