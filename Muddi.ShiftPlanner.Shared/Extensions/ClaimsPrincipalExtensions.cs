using System.Security.Claims;

namespace Muddi.ShiftPlanner.Shared;

public static class ClaimsPrincipalExtensions
{
	public static string GetFullName(this ClaimsPrincipal cp)
	{
		return cp.Identity.Name;
	}
}