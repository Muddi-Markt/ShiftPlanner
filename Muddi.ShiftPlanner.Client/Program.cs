using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Muddi.ShiftPlanner.Client;
using Muddi.ShiftPlanner.Client.Services;
using Muddi.ShiftPlanner.Shared.BlazorWASM;
using Toolbelt.Blazor.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddRadzen();
builder.Services.AddOauthConnect(builder.Configuration.GetRequiredSection("OAuth"));
builder.Services.AddShiftApiExtensions(builder.Configuration.GetRequiredSection("ShiftApi"));
builder.Services.AddScoped<ShiftService>();
builder.Services.AddBlazorDownloadFile();

builder.Services.AddPWAUpdater();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();