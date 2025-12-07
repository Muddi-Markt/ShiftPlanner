using FastEndpoints;
using Void = FastEndpoints.Void;

namespace Muddi.ShiftPlanner.Server.Api;

public static class FastEndpointResponseSenderExtensions
{
	extension<TRequest, TResponse>(ResponseSender<TRequest, TResponse> responseSender)
		where TRequest : notnull, new()
		where TResponse : new()
	{
		public Task<Void> LockedAsync(string reason, CancellationToken ct)
			=> responseSender.StringAsync(reason, StatusCodes.Status423Locked, cancellation: ct);

		public Task<Void> BadRequestAsync(string reason, CancellationToken ct)
			=> responseSender.StringAsync(reason, 400, cancellation: ct);

		public Task<Void> ForbiddenAsync(string reason, CancellationToken ct)
			=> responseSender.StringAsync(reason, StatusCodes.Status403Forbidden, cancellation: ct);


		//I would prefer a 204 status code, but as Refit has an issue so we have to return 200: https://github.com/reactiveui/refit/issues/1128
		public Task<Void> NoContentWith200Async(CancellationToken ct)
			=> responseSender.OkAsync(ct);


		public Task<Void> NotFoundAsync(string idName, CancellationToken ct)
			=> responseSender.StringAsync($"{idName} does not exist", StatusCodes.Status404NotFound, cancellation: ct);

		public Task<Void> ConflictAsync(string reason, CancellationToken ct)
			=> responseSender.StringAsync(reason, StatusCodes.Status409Conflict, cancellation: ct);
	}
}