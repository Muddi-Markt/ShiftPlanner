using System.Globalization;
using Radzen;

namespace Muddi.ShiftPlanner.Client.Extensions.ServiceCollection;

public static class RadzenExtensions
{
	public static void AddRadzen(this IServiceCollection services)
	{
		services.AddScoped<DialogService>();
		services.AddScoped<NotificationService>();
		services.AddScoped<TooltipService>();
		services.AddScoped<ContextMenuService>();
		CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("de-DE");
		CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("de-DE");
	}
	
}