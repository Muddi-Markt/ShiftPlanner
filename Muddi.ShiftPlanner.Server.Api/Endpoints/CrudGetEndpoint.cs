﻿using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints;

public abstract class CrudGetEndpoint<TResponse> : CrudEndpoint<DefaultGetRequest, TResponse>
	where TResponse : notnull, new()
{
	public abstract Task<TResponse?> MuddiExecuteAsync(Guid id, CancellationToken ct);

	public override void Configure()
	{
		Options(t =>
		{
			t.Produces<TResponse>();
			t.Produces(StatusCodes.Status404NotFound);
		});
		base.Configure();
	}

	public sealed override async Task HandleAsync(DefaultGetRequest req, CancellationToken ct)
	{
		var val = await MuddiExecuteAsync(req.Id, ct);
		if (val is null)
		{
			await SendNotFoundAsync(ct);
			return;
		}

		Response = val;
	}

	protected CrudGetEndpoint(ShiftPlannerContext database) : base(database)
	{
	}
}