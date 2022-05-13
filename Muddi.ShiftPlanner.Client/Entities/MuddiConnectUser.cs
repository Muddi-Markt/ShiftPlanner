using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using Muddi.ShiftPlanner.Shared.Entities;

namespace Muddi.ShiftPlanner.Client.Entities;

public sealed class MuddiConnectUser : EmployeeBase
{
	private MuddiConnectUser(Guid keycloakId, string name, string email) : base(keycloakId, name)
	{
		EMail = email;
	}

	public string EMail { get; }

	public static MuddiConnectUser CreateFromClaimsPrincipal(ClaimsPrincipal principal)
	{
		// #if DEBUG
		// return new MuddiConnectUser(Guid.NewGuid(), "Maxi Musterfrau", "lol@muddi.org");
		// #endif
		if (principal.Identity is not { IsAuthenticated: true })
			throw new UnauthorizedAccessException("User is not authenticated");
		var keycloakId = principal.FindFirst("sub")?.Value
		                 ?? throw new ArgumentNullException(nameof(principal), "Can't determine keycloak id from principal");
		var name = principal.Identity?.Name
		           ?? throw new ArgumentNullException(nameof(principal), "Can't determine name from principal");
		var email = principal.FindFirst("email")?.Value
		            ?? throw new ArgumentNullException(nameof(principal), "Can't determine e-mail from principal");
		return new MuddiConnectUser(Guid.Parse(keycloakId), name, email);
	}
}