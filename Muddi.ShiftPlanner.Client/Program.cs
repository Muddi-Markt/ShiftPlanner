using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Muddi.ShiftPlanner.Client;
using Muddi.ShiftPlanner.Shared.Api;
using Refit;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");



builder.Services.AddRadzen();
builder.Services.AddMuddiConnect(builder.Configuration.GetSection("MuddiConnect"));

builder.Services
	.AddRefitClient<IMuddiShiftApi>()
	.ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration["MuddiShiftApi:BaseUrl"]));


builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();