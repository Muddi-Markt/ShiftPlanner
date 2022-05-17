using System.Net.Http.Headers;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Mvc;
using Muddi.ShiftPlanner.Shared.Api;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Requests;
using Refit;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRefitClient<IMuddiShiftApi>()
	.ConfigureHttpClient(c =>
	{
		c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
			"eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJPbnFMQnFDRExEN28zUk5jT2c0VUdDNVkxbExrS1ltNTVYVjFTcFlXeWZJIn0.eyJleHAiOjE2NTI4NjA5NDEsImlhdCI6MTY1MjgyNTI1OSwianRpIjoiYWQxY2U1YTQtODdkNi00ZDFjLTlmNWMtZTk1OTYyYzY1MTk2IiwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDoyODA4MC9yZWFsbXMvbXVkZGkiLCJhdWQiOlsic2hpZnQtcGxhbm5lciIsImFjY291bnQiXSwic3ViIjoiYjYyYjI2NDQtZTk5Ni00MTM2LTg1MDctYjczNDc3YzFiNTk1IiwidHlwIjoiQmVhcmVyIiwiYXpwIjoic2hpZnQtcGxhbm5lciIsInNlc3Npb25fc3RhdGUiOiI4Y2FhOWM3Ni1hZjI5LTRlZmYtYWRhMy02MDQ2NjJiOTdjNGQiLCJhY3IiOiIwIiwiYWxsb3dlZC1vcmlnaW5zIjpbIioiXSwicmVhbG1fYWNjZXNzIjp7InJvbGVzIjpbImRlZmF1bHQtcm9sZXMtbXVkZGkiLCJvZmZsaW5lX2FjY2VzcyIsInVtYV9hdXRob3JpemF0aW9uIl19LCJzY29wZSI6Im9wZW5pZCBwcm9maWxlIGVtYWlsIiwic2lkIjoiOGNhYTljNzYtYWYyOS00ZWZmLWFkYTMtNjA0NjYyYjk3YzRkIiwiZW1haWxfdmVyaWZpZWQiOnRydWUsInJvbGVzIjpbImVkaXRvciIsInZpZXdlciIsInN1cGVyLWFkbWluIiwiYWRtaW4iLCJtYW5hZ2UtYWNjb3VudCIsIm1hbmFnZS1hY2NvdW50LWxpbmtzIiwidmlldy1wcm9maWxlIl0sInByZWZlcnJlZF91c2VybmFtZSI6InN1cGVyLWFkbWluQHJlYmxlLmV1IiwiZ2l2ZW5fbmFtZSI6IiIsImVtYWlsIjoic3VwZXItYWRtaW5AcmVibGUuZXUifQ.emMxBhE7RJnAvhQaaozaxayyRgm6Ib1sXfej_DfGj9tKL1uCoPH7Uy4wak6qiIu1QE8lFBynqL_-9IS_SYPqGjrL3lJtgwkzhQaTWx6sgxNBiw0_jA8Zo3oT9AQC2XZ60kyIFiPpCIESbI0xgwnB0dQrmCBZCdLFY4ZWZ52q9ofOL2JOcOMskXYoyBjaRoos4IFH-FdBdiImcmjPgQPYMpUsHqMs6SeXAPq0rQzzp_rWLczKx7HLNZjbXprQGGFQ-He-Jv4JuF6rpG9F9EvjJCc6DHw3sfI474eXIT5XwC9Wde2Z3CZ2ReDaHwkeP8jJePK6zxi2mdBlG04ibQ_wEw");
		c.BaseAddress = new Uri(builder.Configuration["MuddiShiftApi:BaseUrl"]);
	});
var app = builder.Build();

app.MapGet("/", Test);

app.Run();

static async Task<IResult> Test(IMuddiShiftApi shiftApi)
{
	var location = (await shiftApi.GetAllLocations()).First();
	location = await shiftApi.GetLocationById(location.Id);
	int totalShiftsCreated = 0;
	foreach (var container in location.Containers)
	{
		for (int i = 0; i < container.TotalShifts; i++)
		{
			var startTime = container.Start + TimeSpan.FromSeconds(container.Framework.SecondsPerShift * i);
			foreach (var typeCount in container.Framework.ShiftTypeCounts)
			{
				for (int tc = 0; tc < typeCount.Count; tc++)
				{
					var sr = new CreateShiftRequest
					{
						EmployeeKeycloakId = Guid.NewGuid(),
						Start = startTime,
						ShiftTypeId = typeCount.ShiftType.Id
					};
					await shiftApi.CreateShiftForContainer(container.Id, sr);
					totalShiftsCreated++;
				}
			}
		}
	}

	return Results.Ok($"Total shifts created: {totalShiftsCreated}");
	// var shifts = await shiftApi.GetAllShiftsForLocation(Guid.Parse("5d54a02c-3af4-462e-bcde-88c8838f7cab"));
	// return Results.Ok(shifts);
}