using Microsoft.Extensions.Caching.Memory;
using Refit;

namespace Muddi.ShiftPlanner.Server.Api.Services;

public interface IKeycloakService
{
	Task<ApiResponse<GetTokenResponse>> GetToken(GetTokenRequest tokenRequest);
	Task<KeycloakUserRepresentation> GetUserByIdAsync(Guid reqId);
	Task<IEnumerable<KeycloakUserRepresentation>> GetUsers();
}

public record KeycloakRealm(string Value);

public class KeycloakService : IKeycloakService
{
	private readonly IMemoryCache _cache;
	private readonly IKeycloakApi _keycloakApi;
	private readonly string _realm;

	public KeycloakService(IKeycloakApi keycloakApi, IMemoryCache cache, KeycloakRealm realm)
	{
		_keycloakApi = keycloakApi;
		_cache = cache;
		_realm = realm.Value;
	}

	public Task<ApiResponse<GetTokenResponse>> GetToken(GetTokenRequest tokenRequest)
	{
		return _keycloakApi.GetToken(_realm, tokenRequest);
	}

	public Task<KeycloakUserRepresentation> GetUserByIdAsync(Guid reqId)
	{
		return _cache.GetOrCreateAsync<KeycloakUserRepresentation>("users:" + reqId, async _ =>
		{
			var apiResponse = await _keycloakApi.GetUserByIdAsync(_realm, reqId);
			if (apiResponse is { IsSuccessStatusCode: true, Content: not null })
				return apiResponse.Content!;

			return new KeycloakUserRepresentation()
			{
				Email = string.Empty,
				Id = reqId,
				FirstName = reqId.ToString()[..8],
				LastName = "Unknown"
			};
		})!;
	}

	public async Task<IEnumerable<KeycloakUserRepresentation>> GetUsers()
	{
		var apiResponse = await _keycloakApi.GetUsers(_realm);
		if (!apiResponse.IsSuccessStatusCode)
			throw new Exception("Failed to get users: " + (apiResponse.Error?.Message ??
			                                               apiResponse.ReasonPhrase ??
			                                               apiResponse.StatusCode.ToString()));
		return apiResponse.Content ?? [];
	}
}