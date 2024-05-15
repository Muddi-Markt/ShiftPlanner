using System.Data.Common;
using System.Reflection;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Muddi.ShiftPlanner.Server.Api.Extensions;
using Muddi.ShiftPlanner.Server.Api.Services;
using Muddi.ShiftPlanner.Server.Database.Extensions;
using Npgsql;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, configuration) => configuration
	.ReadFrom.Configuration(context.Configuration));

builder.Services.AddCors();
builder.Services.AddFastEndpoints();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocument();
builder.Services.AddAuthenticationMuddiConnect(builder.Configuration);
builder.Services.AddMuddiShiftPlannerContext(builder.Configuration);
builder.Services.AddDatabaseMigrations();
builder.Services.AddMemoryCache();

builder.Services.AddTransient<ExcelService>();

var app = builder.Build();

var assembly = Assembly.GetEntryAssembly()!.GetName();
app.Logger.LogInformation("Start {AssemblyName} v{AssemblyVersion}", assembly.Name, assembly.Version);

var corsOrigins = builder.Configuration.GetSection("Cors").GetSection("Origins").Get<string[]>();

#if DEBUG
app.UseCors(b => b.WithOrigins(corsOrigins).AllowAnyMethod().AllowAnyHeader());
#else
app.UseCors(b => b.WithOrigins(corsOrigins).AllowAnyMethod().AllowAnyHeader());
#endif

app.UseAuthentication();
app.UseAuthorization();


app.UseFastEndpoints();
app.UseOpenApi();
app.UseSwaggerUi(s => s.ConfigureDefaults());

try
{
	app.Run();
}
catch (DbException dbexc)
{
	app.Logger.LogCritical("Database error: {Message}", dbexc.Message);
	app.Logger.LogTrace("Debug: {Dbg}", dbexc.ToString());
}