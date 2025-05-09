using System.Net;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Muddi.ShiftPlanner.Client.Configuration;

namespace Muddi.ShiftPlanner.Client;

public static class WebAssemblyHostBuilderExtensions
{
	public static async Task LoadCustomizationConfigurationAsync(this WebAssemblyHostBuilder builder)
	{
		// read JSON file as a stream for configuration
		var client = new HttpClient() { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
		var requestMessage = new HttpRequestMessage(HttpMethod.Get, "customization/customization.json");
		requestMessage.SetBrowserRequestCache(BrowserRequestCache.NoCache);
		using var response = await client.SendAsync(requestMessage);
		if (response.StatusCode == HttpStatusCode.NotFound)
			throw new ArgumentException("You need to specify a 'customization.json' in the customization directory");
		response.EnsureSuccessStatusCode();
		await using var stream = await response.Content.ReadAsStreamAsync();
		builder.Configuration.AddJsonStream(stream);
		builder.Services.AddOptions<AppCustomization>().BindConfiguration("App");
	}
}