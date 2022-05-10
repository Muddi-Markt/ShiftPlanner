using Refit;

namespace Muddi.ShiftPlanner.Server.Api.Services;

public class KeycloakService
{
	private const string Realm = "muddi";
	private readonly IKeycloakApi _keycloakApi;


	public KeycloakService(IConfiguration configuration)
	{
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

	public Task<KeycloakUserRepresentation> GetUserById(Guid reqId)
	{
		//TODO cache keycloak user in local dictionary
		return _keycloakApi.GetUserById(Realm, reqId);
	}
}