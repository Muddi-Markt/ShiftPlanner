using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Muddi.ShiftPlanner.Client;
using Muddi.ShiftPlanner.Client.Services;
using Muddi.ShiftPlanner.Shared;
using Muddi.ShiftPlanner.Shared.Api;
using Muddi.ShiftPlanner.Shared.BlazorWASM;
using Refit;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddRadzen();
builder.Services.AddMuddiConnect(builder.Configuration.GetRequiredSection("MuddiConnect"));
builder.Services.MuddiShiftApiExtensions(builder.Configuration.GetRequiredSection("MuddiShiftApi"));
builder.Services.AddScoped<ShiftService>();
builder.Services.AddBlazorDownloadFile();


builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();