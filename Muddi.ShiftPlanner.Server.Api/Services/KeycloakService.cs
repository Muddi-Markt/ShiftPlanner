using Microsoft.Extensions.Caching.Memory;
using Muddi.ShiftPlanner.Server.Api.Extensions;
using Refit;

namespace Muddi.ShiftPlanner.Server.Api.Services;

public interface IKeycloakService
{
	Task<ApiResponse<GetTokenResponse>> GetToken(ClientCredentialsTokenRequest tokenRequest);
	GetEmployeeResponse GetUserById(Guid reqId);
	ValueTask<GetEmployeeResponse> GetUserByIdAsync(Guid reqId);
	Task<IEnumerable<GetEmployeeResponse>> GetUsers();
}

public class KeycloakService : IKeycloakService
{
	private readonly IMemoryCache _cache;
	private const string Realm = "muddi";
	private const int TokenRefreshBufferSeconds = 30; // Refresh token 30 seconds before expiration
	private readonly IKeycloakApi _keycloakApi;
	private static readonly SemaphoreSlim TokenSemaphore = new(1, 1);
	private static string? _currentToken;
	private static DateTime _currentTokenExpiresAt;
	private readonly ClientCredentialsTokenRequest _tokenRequest;

	public KeycloakService(IConfiguration configuration, IMemoryCache cache)
	{
		_cache = cache;
		var authority = configuration["MuddiConnect:Authority"];
		var c = configuration.GetSection("MuddiConnect");
		var clientId = c["ClientId"] ?? "shift-planner";
		var clientSecret = c["ClientSecret"] ?? throw new InvalidOperationException("ClientSecret is required");

		if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
			throw new InvalidOperationException("You need to specify ClientId and ClientSecret in MuddiConnect configuration");

		_tokenRequest = new ClientCredentialsTokenRequest(clientId, clientSecret);

		_keycloakApi = RestService.For<IKeycloakApi>(new Uri(authority).GetLeftPart(UriPartial.Authority),
			new RefitSettings
			{
				AuthorizationHeaderValueGetter = GetAdminToken
			});
	}

	private async Task<string> GetAdminToken(HttpRequestMessage message, CancellationToken ct)
	{
		await TokenSemaphore.WaitAsync(ct);
		try
		{
			if (_currentToken is not null && _currentTokenExpiresAt > DateTime.UtcNow)
				return _currentToken;

			var apiRes = await _keycloakApi.GetToken(_tokenRequest);
			if (!apiRes.IsSuccessStatusCode)
				throw new Exception($"Failed to get token: {apiRes.Error?.Content ?? apiRes.ReasonPhrase}");

			if (apiRes.Content is not { } res)
				throw apiRes.Error!;

			_currentToken = res.AccessToken;
			_currentTokenExpiresAt = DateTime.UtcNow.AddSeconds(res.ExpiresIn - TokenRefreshBufferSeconds);

			return _currentToken;
		}
		finally
		{
			TokenSemaphore.Release();
		}
	}

	public Task<ApiResponse<GetTokenResponse>> GetToken(ClientCredentialsTokenRequest tokenRequest)
	{
		return _keycloakApi.GetToken(tokenRequest);
	}

	public GetEmployeeResponse GetUserById(Guid reqId)
	{
		if (!_cache.TryGetValue(reqId, out GetEmployeeResponse response))
		{
			var apiResponse = _keycloakApi.GetUserByIdAsync(Realm, reqId).GetAwaiter().GetResult();
			var keycloakUser = apiResponse.Content;

			if (apiResponse.IsSuccessStatusCode && keycloakUser is not null)
			{
				response = keycloakUser.MapToEmployeeResponse();
			}
			else
			{
				response = new()
				{
					Email = string.Empty,
					Id = reqId,
					UserName = "Unknown User",
					FirstName = reqId.ToString()[..8],
					LastName = "Unknown"
				};
			}
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
			{
				response = keycloakUser.MapToEmployeeResponse();
			}
			else
			{
				response = new()
				{
					Email = string.Empty,
					Id = reqId,
					UserName = "Unknown User",
					FirstName = reqId.ToString()[..8],
					LastName = "Unknown"
				};
			}
			_cache.Set(reqId, response, TimeSpan.FromHours(1));
		}

		return response;
	}

	public async Task<IEnumerable<GetEmployeeResponse>> GetUsers()
	{
		var apiResponse = await _keycloakApi.GetUsers(Realm);
		if (!apiResponse.IsSuccessStatusCode)
			throw new Exception($"Failed to get users: {apiResponse.Error?.Message ?? apiResponse.ReasonPhrase ?? apiResponse.StatusCode}");

		if (apiResponse.Content is null)
			return Enumerable.Empty<GetEmployeeResponse>();

		return apiResponse.Content.Select(u => u.MapToEmployeeResponse());
	}
}