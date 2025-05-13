using System.Text.Json.Serialization;
using Refit;

namespace Muddi.ShiftPlanner.Server.Api.Services;

public interface IKeycloakApi
{
	[Get("/admin/realms/{realm}/users/{userId}")]
	[Headers("Authorization: Bearer")]
	Task<ApiResponse<KeycloakUserRepresentation>> GetUserByIdAsync(string realm, Guid userId);

	[Get("/admin/realms/{realm}/users")]
	[Headers("Authorization: Bearer")]
	Task<ApiResponse<ICollection<KeycloakUserRepresentation>>> GetUsers(string realm, int max = -1);

	[Post("/realms/muddi/protocol/openid-connect/token")]
	Task<ApiResponse<GetTokenResponse>> GetToken([Body(BodySerializationMethod.UrlEncoded)] ClientCredentialsTokenRequest request);
}

public record ClientCredentialsTokenRequest(
	[property: JsonPropertyName("client_id")] string ClientId,
	[property: JsonPropertyName("client_secret")] string ClientSecret,
	[property: JsonPropertyName("grant_type")] string GrantType = "client_credentials"
);

public record GetTokenResponse
{
	[JsonPropertyName("access_token")]
	public string AccessToken { get; init; } = string.Empty;

	[JsonPropertyName("expires_in")]
	public int ExpiresIn { get; init; }

	[JsonPropertyName("token_type")]
	public string TokenType { get; init; } = "Bearer";
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