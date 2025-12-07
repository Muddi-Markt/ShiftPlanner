using System.Globalization;
using FastEndpoints;
using FluentValidation.Results;
using Mapster;
using Microsoft.AspNetCore.Authentication;
using Muddi.ShiftPlanner.Server.Api.Exceptions;
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
		CrudConfigure();
		//If no roles are specified, add admin role as default
		if (Definition.AllowedRoles?.Count == 0)
			Roles(ApiRoles.Admin);
// #if DEBUG
// 		AllowAnonymous();
// #else
// 		#error Don't AllowAnonymous ever at default
// #endif
		Options(t => t.Produces<TResponse>());
#if !DEBUG
		Throttle(500, 60);
#endif
	}
	protected async Task<bool> SendErrorIfValidationFailure(ValidationFailure? validationFailure)
	{
		if (validationFailure is null) return false;
		var statusCode = validationFailure switch
		{
			AlreadyShiftAtGivenTimeFailure => StatusCodes.Status409Conflict,
			_ => StatusCodes.Status400BadRequest
		};

		ValidationFailures.Add(validationFailure);
		await Send.ErrorsAsync(statusCode);
		return true;
	}
}