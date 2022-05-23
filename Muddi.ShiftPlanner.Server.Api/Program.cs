using System.Data.Common;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Muddi.ShiftPlanner.Server.Api.Extensions;
using Muddi.ShiftPlanner.Server.Database.Extensions;
using Npgsql;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, configuration) => configuration
	.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3} {SourceContext}] {Message:lj}{NewLine}{Exception}")
	.Enrich.FromLogContext()
	.MinimumLevel.Warning());

builder.Services.AddCors();
builder.Services.AddFastEndpoints();
builder.Services.AddSwaggerDoc();
builder.Services.AddAuthenticationMuddiConnect(builder.Configuration);
builder.Services.AddMuddiShiftPlannerContext(builder.Configuration);
builder.Services.AddDatabaseMigrations();
builder.Services.AddMemoryCache();

var app = builder.Build();

#if DEBUG
app.UseCors(b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
#else
app.UseCors(b => b.WithOrigins("https://shift.muddi.reble.eu").AllowAnyMethod().AllowAnyHeader());
#endif

app.UseAuthentication();
app.UseAuthorization();


app.UseFastEndpoints();
app.UseOpenApi();
app.UseSwaggerUi3(s => s.ConfigureDefaults());

try
{
	app.Run();
}
catch (DbException dbexc)
{
	app.Logger.LogCritical("Database error: {Message}", dbexc.Message);
	app.Logger.LogTrace("Debug: {Dbg}", dbexc.ToString());
}