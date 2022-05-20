using System.Globalization;
using FastEndpoints;
using FluentValidation.Results;
using Mapster;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Shared.Contracts.v1;
using Serilog.Parsing;

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
		Roles(ApiRoles.Admin); //Can be replaced in CrudConfigure() but default is Admin
		CrudConfigure();
// #if DEBUG
		// AllowAnonymous();
// #endif
		Options(t => t.Produces<TResponse>());
#if !DEBUG
		Throttle(500, 60);
#endif
	}


	//We don't need to pass the CancellationToken as the methods itself have it already
	protected Task SendNotFoundAsync(string idName)
	{
		return SendStringAsync($"{idName} does not exist", StatusCodes.Status404NotFound);
	}

	protected Task SendConflictAsync(string reason)
	{
		return SendStringAsync(reason, StatusCodes.Status409Conflict);
	}

	protected Task SendLockedAsync(string reason)
	{
		return SendStringAsync(reason, StatusCodes.Status423Locked);
	}

	protected Task SendBadRequest(string reason)
	{
		return SendStringAsync(reason, 400);
	}
	
	protected Task SendForbiddenAsync(string reason)
	{
		return SendStringAsync(reason, StatusCodes.Status403Forbidden);
	}

	protected async Task<bool> SendErrorIfValidationFailure(ValidationFailure? validationFailure)
	{
		if (validationFailure is null) return false;
		ValidationFailures.Add(validationFailure);
		await SendErrorsAsync();
		return true;
	}
}