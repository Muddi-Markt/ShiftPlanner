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
		var sub = principal.FindFirst("sub")?.Value;
		if (string.IsNullOrEmpty(sub))
		{
			throw new ArgumentException(
				"The JWT token does not contain a 'sub' claim. Please ensure the Keycloak client has a protocol mapper configured to include the 'sub' claim.",
				nameof(principal));
		}
		return Guid.Parse(sub);
	}

	public static string GetEMail(this ClaimsPrincipal principal)
	{
		return principal.FindFirst("email")?.Value
			   ?? throw new ArgumentNullException(nameof(principal), "Can't determine e-mail from principal");
	}
}