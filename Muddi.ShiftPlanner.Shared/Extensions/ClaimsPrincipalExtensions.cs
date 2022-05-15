using System.Security.Claims;

namespace Muddi.ShiftPlanner.Shared;

public static class ClaimsPrincipalExtensions
{
	public static string GetFullName(this ClaimsPrincipal principal)
	{
		return principal.Identity?.Name ?? throw new ArgumentNullException(nameof(principal), "Can't determine name from principal");
	}

	public static Guid GetKeycloakId(this ClaimsPrincipal principal)
	{
		var sub = principal.FindFirst("sub")?.Value
		          ?? throw new ArgumentNullException(nameof(principal), "Can't determine keycloak id from principal");
		return Guid.Parse(sub);
	}

	public static string GetEMail(this ClaimsPrincipal principal)
	{
		return principal.FindFirst("email")?.Value
		       ?? throw new ArgumentNullException(nameof(principal), "Can't determine e-mail from principal");
	}
}