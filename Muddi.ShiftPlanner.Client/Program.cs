using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Muddi.ShiftPlanner.Client;
using Muddi.ShiftPlanner.Client.Extensions.ServiceCollection;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");



builder.Services.AddRadzen();
builder.Services.AddMuddiConnect(builder.Configuration.GetSection("MuddiConnect"));


builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();