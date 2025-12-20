using System.Text.Json.Serialization;
using Refit;

namespace Muddi.ShiftPlanner.Server.Api.Services;

public interface IKeycloakApi
{
	[Get("/admin/realms/{realm}/users/{userId}")]
	Task<ApiResponse<KeycloakUserRepresentation>> GetUserByIdAsync(string realm, Guid userId);

	[Get("/admin/realms/{realm}/users")]
	Task<ApiResponse<ICollection<KeycloakUserRepresentation>>> GetUsers(string realm, int max = -1);

	[Post("/realms/{realm}/protocol/openid-connect/token")]
	Task<ApiResponse<GetTokenResponse>> GetToken(string realm,
		[Body(BodySerializationMethod.UrlEncoded)] GetTokenRequest request);
}

public class KeycloakUserRepresentation
{
	[JsonPropertyName("email")] public string Email { get; set; }
	[JsonPropertyName("username")] public string Username { get; set; }
	[JsonPropertyName("firstName")] public string? FirstName { get; set; }
	[JsonPropertyName("lastName")] public string? LastName { get; set; }
	[JsonPropertyName("enabled")] public bool Enabled { get; set; }
	[JsonPropertyName("id")] public string? Id { get; set; }
}

public class GetTokenResponse
{
	[JsonPropertyName("access_token")] public string AccessToken { get; set; }

	[JsonPropertyName("expires_in")] public int ExpiresIn { get; set; }
}

public class GetTokenRequest
{
	public GetTokenRequest(string username, string password, string clientId)
	{
		Username = username;
		Password = password;
		ClientId = clientId;
	}

	[AliasAs("username")] public string Username { get; init; }
	[AliasAs("password")] public string Password { get; init; }
	[AliasAs("client_id")] public string ClientId { get; }

	[AliasAs("grant_type")] public string GrantType => "password";
}