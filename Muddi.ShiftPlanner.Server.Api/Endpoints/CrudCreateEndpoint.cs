using FastEndpoints;
using Muddi.ShiftPlanner.Server.Api.Contracts.Requests;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses.Frameworks;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints;

public abstract class CrudCreateEndpoint<TRequest, TResponse, TGetEndpoint> : CrudEndpoint<TRequest, TResponse>
	where TRequest : notnull, new()
	where TResponse : IMuddiResponse, new()
	where TGetEndpoint : IEndpoint
{
	public abstract Task<TResponse?> CrudExecuteAsync(TRequest req, CancellationToken ct);

	public override void Configure()
	{
		Options(t => { t.Produces<DefaultCreateResponse>(StatusCodes.Status201Created); });
		base.Configure();
	}

	public sealed override async Task HandleAsync(TRequest req, CancellationToken ct)
	{
		var resp = await CrudExecuteAsync(req, ct);
		if (resp is null)
			return;

		await SendCreatedAtAsync<TGetEndpoint>(new { resp.Id }, resp, cancellation: ct);
	}

	protected CrudCreateEndpoint(ShiftPlannerContext database) : base(database)
	{
	}
}