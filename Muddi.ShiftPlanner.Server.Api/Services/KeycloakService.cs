using Microsoft.Extensions.Caching.Memory;
using Muddi.ShiftPlanner.Server.Api.Extensions;
using Refit;

namespace Muddi.ShiftPlanner.Server.Api.Services;

public interface IKeycloakService
{
	Task<ApiResponse<GetTokenResponse>> GetToken(GetTokenRequest tokenRequest);
	GetEmployeeResponse GetUserById(Guid reqId);
	ValueTask<GetEmployeeResponse> GetUserByIdAsync(Guid reqId);
	Task<IEnumerable<GetEmployeeResponse>> GetUsers();
}

public class KeycloakService : IKeycloakService
{
	private readonly IMemoryCache _cache;
	private readonly IKeycloakApi _keycloakApi;
	private const string Realm = "muddi";

	public KeycloakService(IKeycloakApi keycloakApi, IMemoryCache cache)
	{
		_keycloakApi = keycloakApi;
		_cache = cache;
	}

	public Task<ApiResponse<GetTokenResponse>> GetToken(GetTokenRequest tokenRequest)
	{
		return _keycloakApi.GetToken(Realm, tokenRequest);
	}

	public GetEmployeeResponse GetUserById(Guid reqId)
	{
		if (!_cache.TryGetValue(reqId, out GetEmployeeResponse response))
		{
			var apiResponse = _keycloakApi.GetUserByIdAsync(Realm, reqId).GetAwaiter().GetResult();
			var keycloakUser = apiResponse.Content;

			if (apiResponse.IsSuccessStatusCode && keycloakUser is not null)
				response = keycloakUser.MapToEmployeeResponse();
			else
				response = new()
				{
					Email = string.Empty,
					Id = reqId,
					UserName = "Unknown User",
					FirstName = reqId.ToString()[..8],
					LastName = "Unknown"
				};
			_cache.Set(reqId, response, TimeSpan.FromHours(1));
		}

		return response;
	}

	public async ValueTask<GetEmployeeResponse> GetUserByIdAsync(Guid reqId)
	{
		if (!_cache.TryGetValue(reqId, out GetEmployeeResponse response))
		{
			var apiResponse = await _keycloakApi.GetUserByIdAsync(Realm, reqId);
			var keycloakUser = apiResponse.Content;

			if (apiResponse.IsSuccessStatusCode && keycloakUser is not null)
				response = keycloakUser.MapToEmployeeResponse();
			else
				response = new()
				{
					Email = string.Empty,
					Id = reqId,
					UserName = "Unknown User",
					FirstName = reqId.ToString()[..8],
					LastName = "Unknown"
				};
			_cache.Set(reqId, response, TimeSpan.FromHours(1));
		}

		return response;
	}

	public async Task<IEnumerable<GetEmployeeResponse>> GetUsers()
	{
		var apiResponse = await _keycloakApi.GetUsers(Realm);
		if (!apiResponse.IsSuccessStatusCode)
			throw new Exception("Failed to get users: " + (apiResponse.Error?.Message ??
			                                               apiResponse.ReasonPhrase ??
			                                               apiResponse.StatusCode.ToString()));
		return apiResponse.Content is null
			? Enumerable.Empty<GetEmployeeResponse>()
			: apiResponse.Content.Select(u => u.MapToEmployeeResponse());
	}
}