using Muddi.ShiftPlanner.Services.Alerting.Extensions;
using Muddi.ShiftPlanner.Services.Alerting.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<AutomaticShiftCheckingService>();
builder.Services.AddTelegramBot(builder.Configuration);
builder.Services.AddSingleton<LocationsService>();
builder.Services.AddSingleton<MuddiService>();
builder.Services.AddMemoryCache();

var app = builder.Build();


app.Run();