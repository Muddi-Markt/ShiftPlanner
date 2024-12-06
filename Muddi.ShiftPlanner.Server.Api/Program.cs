using System.Data.Common;
using System.Reflection;
using FastEndpoints;
using FastEndpoints.Swagger;
using Muddi.ShiftPlanner.Server.Api.Extensions;
using Muddi.ShiftPlanner.Server.Api.Services;
using Muddi.ShiftPlanner.Server.Database.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, configuration) => configuration
	.ReadFrom.Configuration(context.Configuration));

builder.Services.AddCors();
builder.Services.AddFastEndpoints().SwaggerDocument();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthenticationMuddiConnect(builder.Configuration);
builder.Services.AddMuddiShiftPlannerContext(builder.Configuration);
builder.Services.AddDatabaseMigrations();
builder.Services.AddMemoryCache();

builder.Services.AddResponseCompression(options => { options.EnableForHttps = true; });


builder.Services.AddTransient<ExcelService>();

var app = builder.Build();

var assembly = Assembly.GetEntryAssembly()!.GetName();
app.Logger.LogInformation("Start {AssemblyName} v{AssemblyVersion}", assembly.Name, assembly.Version);

var corsOrigins = builder.Configuration.GetSection("Cors").GetSection("Origins").Get<string[]>();
if (corsOrigins is null || corsOrigins.Length == 0)
	throw new ArgumentException("You have to set Cors__Origins array in settings");

app.UseCors(b => b.WithOrigins(corsOrigins).AllowAnyMethod().AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.UseResponseCompression();
app.UseFastEndpoints()
	.UseSwaggerGen();


try
{
	app.Run();
}
catch (DbException dbexc)
{
	app.Logger.LogCritical("Database error: {Message}", dbexc.Message);
	app.Logger.LogTrace("Debug: {Dbg}", dbexc.ToString());
}