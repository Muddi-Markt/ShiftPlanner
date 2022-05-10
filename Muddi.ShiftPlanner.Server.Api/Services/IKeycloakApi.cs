using System.Text.Json.Serialization;
using Refit;

namespace Muddi.ShiftPlanner.Server.Api.Services;

public interface IKeycloakApi
{
	[Get("/admin/realms/{realm}/users/{userId}")]
	[Headers("Authorization: Bearer")]
	Task<KeycloakUserRepresentation> GetUserById(string realm, Guid userId);
	
	[Post("/realms/{realm}/protocol/openid-connect/token")]
	Task<GetTokenResponse> GetToken(string realm, [Body(BodySerializationMethod.UrlEncoded)] GetTokenRequest request);

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
	public GetTokenRequest(string username, string password)
	{
		this.Username = username;
		this.Password = password;
	}

	[AliasAs("username")] public string Username { get; init; }
	[AliasAs("password")] public string Password { get; init; }
	[AliasAs("client_id")] public string ClientId => "admin-cli";
	[AliasAs("grant_type")] public string GrantType => "password";
}