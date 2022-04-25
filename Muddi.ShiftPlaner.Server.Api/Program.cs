using FastEndpoints;
using FastEndpoints.Security;
using Muddi.ShiftPlaner.Server.Api.ExtensionMethods;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, configuration) => configuration.WriteTo.Console().MinimumLevel.Debug());

builder.Services.AddFastEndpoints();
builder.Services.AddAuthenticationMuddiConnect(builder.Configuration);

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();

app.Run();