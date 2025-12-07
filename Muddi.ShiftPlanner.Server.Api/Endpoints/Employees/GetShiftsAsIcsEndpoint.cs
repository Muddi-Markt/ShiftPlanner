using System.Text;
using FastEndpoints;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Api.Extensions;
using Muddi.ShiftPlanner.Server.Api.Services;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Server.Database.Entities;
using Muddi.ShiftPlanner.Shared;
using Muddi.ShiftPlanner.Shared.Contracts.v1;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Employees;

public class GetShiftsAsIcsEndpoint : Endpoint<GetShiftsFromEmployeeRequest>
{
	private readonly ShiftPlannerContext _database;
	private readonly IKeycloakService _keycloakService;

	public GetShiftsAsIcsEndpoint(ShiftPlannerContext database, IKeycloakService keycloakService)
	{
		_database = database;
		_keycloakService = keycloakService;
	}

	public override void Configure()
	{
		Get("/employees/shifts-ics");
		Roles(ApiRoles.Viewer, ApiRoles.Editor); //Can be replaced in CrudConfigure() but default is Admin
		Options(t => t.Produces(200, contentType: "text/calendar"));
		Throttle(20, 60);
	}

	public override async Task HandleAsync(GetShiftsFromEmployeeRequest request, CancellationToken ct)
	{
		var count = request.Count ?? -1;
		var id = request.Id ?? User.GetKeycloakId();
		if (count == 0)
		{
			await Send.ForbiddenAsync(ct);
			return;
		}

		IQueryable<ShiftEntity> b = _database.Shifts
			.Where(s => s.Type.Season.Id == request.SeasonId)
			.Include(s => s.Type)
			.Include(s => s.ShiftContainer)
			.ThenInclude(c => c.Location)
			.Where(s => s.EmployeeKeycloakId == id)
			.OrderBy(s => s.Start);
		if (count > 0)
			b = b.Take(count);

		var shifts = await b.ToListAsync(cancellationToken: ct);
		if (!shifts.Any())
		{
			await Send.NoContentWith200Async(ct);
			return;
		}

		var calendar = shifts.ToICalCalendar();
		var bytes = calendar.ToByteArray();

		await Send.BytesAsync(bytes, fileName: "muddi-calendar.ics", contentType: "text/calendar", cancellation: ct);
	}
}