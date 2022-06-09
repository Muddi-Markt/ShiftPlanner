using System.Net.Http.Headers;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Mvc;
using Muddi.ShiftPlanner.Services.Alerting.Services;
using Muddi.ShiftPlanner.Shared.Api;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Requests;
using Refit;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<AutomaticShiftCheckingService>();
var app = builder.Build();



app.Run();