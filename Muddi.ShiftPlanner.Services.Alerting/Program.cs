using Microsoft.AspNetCore.Mvc;
using Muddi.ShiftPlanner.Shared.Api;
using Refit;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRefitClient<IMuddiShiftApi>()
	.ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration["MuddiShiftApi:BaseUrl"]));
var app = builder.Build();

app.MapGet("/", Test);

app.Run();

static async Task<IResult> Test(IMuddiShiftApi shiftApi)
{
	var shifts = await shiftApi.GetAllShiftsForLocation(Guid.Parse("5d54a02c-3af4-462e-bcde-88c8838f7cab"));
	return Results.Ok(shifts);
}