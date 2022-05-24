using Microsoft.AspNetCore.Components;

namespace Muddi.ShiftPlanner.Client;

public static class NavigationManagerExtensions
{

	public static void NavigateToLogin(this NavigationManager navigation)
	{
		navigation.NavigateTo($"authentication/login?returnUrl={Uri.EscapeDataString(navigation.Uri)}");
	}
	
	public static void NavigateToLogout(this NavigationManager navigation)
	{
		navigation.NavigateTo($"authentication/logout");
	}
	
}