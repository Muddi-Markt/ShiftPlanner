using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Muddi.ShiftPlanner.Server.Api.ExtensionMethods;
using Muddi.ShiftPlanner.Server.Database.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, configuration) => configuration.WriteTo.Console().MinimumLevel.Debug());

builder.Services.AddCors();
builder.Services.AddFastEndpoints();
builder.Services.AddSwaggerDoc();
builder.Services.AddAuthenticationMuddiConnect(builder.Configuration);
builder.Services.AddMuddiShiftPlannerContext(builder.Configuration);
builder.Services.AddDatabaseMigrations();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors(b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseFastEndpoints();
app.UseOpenApi();
app.UseSwaggerUi3(s => s.ConfigureDefaults());

app.Run();