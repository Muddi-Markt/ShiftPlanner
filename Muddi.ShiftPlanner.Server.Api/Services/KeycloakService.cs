using Microsoft.Extensions.Caching.Memory;
using Refit;

namespace Muddi.ShiftPlanner.Server.Api.Services;

public interface IKeycloakService
{
	Task<string> GetToken();
	Task<GetEmployeeResponse> GetUserById(Guid reqId);
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
		_tokenRequest = new GetTokenRequest(keycloakUser, keycloakPass);
		_keycloakApi = RestService.For<IKeycloakApi>(new Uri(authority).GetLeftPart(UriPartial.Authority),
			new RefitSettings
			{
				AuthorizationHeaderValueGetter = GetToken
			});
	}

	private static readonly SemaphoreSlim TokenSemaphore = new(1, 1);
	private static string? _currentToken;
	private static DateTime _currentTokenExpiresAt;
	private readonly GetTokenRequest _tokenRequest;

	public async Task<string> GetToken()
	{
		await TokenSemaphore.WaitAsync();

		try
		{
			if (_currentToken is not null && _currentTokenExpiresAt > DateTime.UtcNow) return _currentToken;
			var res = await _keycloakApi.GetToken(Realm, _tokenRequest);
			_currentToken = res.AccessToken;
			_currentTokenExpiresAt = DateTime.UtcNow.AddSeconds(res.ExpiresIn);
			return _currentToken;
		}
		finally
		{
			TokenSemaphore.Release();
		}
	}

	public async Task<GetEmployeeResponse> GetUserById(Guid reqId)
	{
		if (!_cache.TryGetValue(reqId, out GetEmployeeResponse response))
		{
			var apiResponse = await _keycloakApi.GetUserById(Realm, reqId);
			var keycloakUser = apiResponse.Content;

			if (apiResponse.IsSuccessStatusCode)
				response = new GetEmployeeResponse
				{
					Email = keycloakUser!.Email,
					Id = Guid.Parse(keycloakUser.Id ?? throw new InvalidOperationException("Keycloak Id is null")),
					UserName = keycloakUser.Username,
					FirstName = keycloakUser.FirstName,
					LastName = keycloakUser.LastName
				};
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
}