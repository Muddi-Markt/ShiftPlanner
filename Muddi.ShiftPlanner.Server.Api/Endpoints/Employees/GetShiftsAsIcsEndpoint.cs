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
			await SendStringAsync("count is 0", 400, cancellation: ct);
			return;
		}

		bool isRequestUserOrAdmin = id == User.GetKeycloakId() || User.IsInRole(ApiRoles.Admin);
		if (!isRequestUserOrAdmin)
		{
			await SendForbiddenAsync(ct);
			return;
		}

		IQueryable<ShiftEntity> b = _database.Shifts
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
			await SendNoContentAsync(ct);
			return;
		}

		var calendar = shifts.ToICalCalendar();
		var bytes = calendar.ToByteArray();

		await SendBytesAsync(bytes, fileName: "muddi-calendar.ics", contentType: "text/calendar", cancellation: ct);
	}
}