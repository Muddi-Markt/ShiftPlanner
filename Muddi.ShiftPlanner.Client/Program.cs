using System.Net;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Muddi.ShiftPlanner.Client;
using Muddi.ShiftPlanner.Client.Services;
using Muddi.ShiftPlanner.Shared.BlazorWASM;
using Toolbelt.Blazor.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

await LoadCustomizationConfigurationAsync(builder);

builder.Services.AddRadzen();
builder.Services.AddOauthConnect(builder.Configuration.GetRequiredSection("OAuth"));
builder.Services.AddShiftApiExtensions(builder.Configuration.GetRequiredSection("ShiftApi"));
builder.Services.AddScoped<ShiftService>();
builder.Services.AddBlazorDownloadFile();

builder.Services.AddPWAUpdater();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();

static async Task LoadCustomizationConfigurationAsync(WebAssemblyHostBuilder builder)
{
	// read JSON file as a stream for configuration
	var client = new HttpClient() { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
	using var response = await client.GetAsync("customization/customization.json");
	if (response.StatusCode == HttpStatusCode.NotFound)
		throw new ArgumentException("You need to specify a 'customization.json' in the customization directory");
	response.EnsureSuccessStatusCode();
	await using var stream = await response.Content.ReadAsStreamAsync();
	builder.Configuration.AddJsonStream(stream);
	builder.Services.AddOptions<AppCustomization>().BindConfiguration("App");
}

public class AppCustomization
{
	public string Title { get; init; }
	public string Subtitle { get; init; }
	public string Contact { get; init; }
	public string ContactHref => (Contact.Contains('@') ? "mailto:" + Contact : "#");
}