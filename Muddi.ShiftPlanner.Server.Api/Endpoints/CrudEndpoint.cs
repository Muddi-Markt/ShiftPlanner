﻿using System.Globalization;
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

	private static TResponse Empty = new();

	protected Task SendNoContent()
	{
		//I would prefer this but as Refit has an issue so we have to return 200: https://github.com/reactiveui/refit/issues/1128
		// return SendAsync(Empty, StatusCodes.Status204NoContent);
		return SendAsync(Empty);
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
		var statusCode = validationFailure switch
		{
			AlreadyShiftAtGivenTimeFailure => StatusCodes.Status409Conflict,
			_ => StatusCodes.Status400BadRequest
		};

		ValidationFailures.Add(validationFailure);
		await SendErrorsAsync(statusCode);
		return true;
	}
}