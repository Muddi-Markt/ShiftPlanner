using Microsoft.Extensions.Caching.Memory;
using Muddi.ShiftPlanner.Server.Api.Extensions;
using Refit;

namespace Muddi.ShiftPlanner.Server.Api.Services;

public interface IKeycloakService
{
	Task<ApiResponse<GetTokenResponse>> GetToken(GetTokenRequest tokenRequest);
	Task<GetEmployeeResponse> GetUserById(Guid reqId);
	Task<IEnumerable<GetEmployeeResponse>> GetUsers();
}

public class KeycloakService : IKeycloakService
{
	private readonly IMemoryCache _cache;
	private const string Realm = "muddi";
	private readonly IKeycloakApi _keycloakApi;


	public KeycloakService(IConfiguration configuration, IMemoryCache cache)
	{
		_cache = cache;
		var authority = configuration["MuddiConnect:Authority"];


		var c = configuration.GetSection("MuddiConnect");
		var keycloakUser = c["AdminUser"];
		var keycloakPass = c["AdminPassword"];

		if (string.IsNullOrEmpty(keycloakUser) || string.IsNullOrEmpty(keycloakPass))
			throw new InvalidOperationException("You need to specify AdminUser and AdminPassword in MuddiConnect");
		_tokenRequest = new GetTokenRequest(keycloakUser, keycloakPass, "admin-cli");
		_keycloakApi = RestService.For<IKeycloakApi>(new Uri(authority).GetLeftPart(UriPartial.Authority),
			new RefitSettings
			{
				AuthorizationHeaderValueGetter = GetAdminToken
			});
	}

	private static readonly SemaphoreSlim TokenSemaphore = new(1, 1);
	private static string? _currentToken;
	private static DateTime _currentTokenExpiresAt;
	private readonly GetTokenRequest _tokenRequest;

	private async Task<string> GetAdminToken()
	{
		await TokenSemaphore.WaitAsync();
		try
		{
			if (_currentToken is not null && _currentTokenExpiresAt > DateTime.UtcNow) return _currentToken;
			var apiRes = await GetToken(_tokenRequest);
			if (apiRes.Content is not { } res) throw apiRes.Error!;
			_currentToken = res.AccessToken;
			_currentTokenExpiresAt = DateTime.UtcNow.AddSeconds(res.ExpiresIn);
			return _currentToken;
		}
		finally
		{
			TokenSemaphore.Release();
		}
	}

	public Task<ApiResponse<GetTokenResponse>> GetToken(GetTokenRequest tokenRequest)
	{
		return _keycloakApi.GetToken(Realm, tokenRequest);
	}

	public async Task<GetEmployeeResponse> GetUserById(Guid reqId)
	{
		if (!_cache.TryGetValue(reqId, out GetEmployeeResponse response))
		{
			var apiResponse = await _keycloakApi.GetUserById(Realm, reqId);
			var keycloakUser = apiResponse.Content;

			if (apiResponse.IsSuccessStatusCode && keycloakUser is not null)
				response = keycloakUser.MapToEmployeeResponse();
			else
				response = new()
				{
					Email = string.Empty,
					Id = reqId,
					UserName = "Unknown User",
					FirstName = "Unknown",
					LastName = "Unknown"
				};
			_cache.Set(reqId, response, TimeSpan.FromHours(1));
		}

		return response;
	}

	public async Task<IEnumerable<GetEmployeeResponse>> GetUsers()
	{
		var apiResponse = await _keycloakApi.GetUsers(Realm);
		return apiResponse.IsSuccessStatusCode && apiResponse.Content is not null
			? apiResponse.Content.Select(u => u.MapToEmployeeResponse())
			: Enumerable.Empty<GetEmployeeResponse>();
	}
}