using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Muddi.ShiftPlanner.Shared.Api;
using Refit;

namespace Muddi.ShiftPlanner.Shared.BlazorWASM;

public static partial class ServiceCollectionExtensions
{
	public static void MuddiShiftApiExtensions(this IServiceCollection services, IConfigurationSection configuration)
	{
		var baseUri = configuration["BaseUrl"] ?? throw new ArgumentNullException(nameof(configuration),"Failed to get BaseUrl from configuration");
		services.AddScoped<AuthorizationMessageHandler>();
		services.AddRefitClient<IMuddiShiftApi>()
			.ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUri))
			.AddHttpMessageHandler(sp => sp
				.GetRequiredService<AuthorizationMessageHandler>()
				.ConfigureHandler(authorizedUrls: new[] { baseUri }));
	}
}