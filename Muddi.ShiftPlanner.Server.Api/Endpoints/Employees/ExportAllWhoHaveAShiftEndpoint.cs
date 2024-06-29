using System.Collections.Immutable;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Api.Services;
using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Employees;

public class
	ExportAllWhoHaveAShiftEndpoint : CrudEndpoint<ExportAllWhoHaveAShiftRequest, ExportAllWhoHaveAShiftResponse>
{
	private readonly IKeycloakService _keycloakService;

	public ExportAllWhoHaveAShiftEndpoint(ShiftPlannerContext database, IKeycloakService keycloakService) :
		base(database)
	{
		_keycloakService = keycloakService;
	}

	protected override void CrudConfigure()
	{
		Get("/employees/at-least-one-shift");
	}

	public override async Task HandleAsync(ExportAllWhoHaveAShiftRequest req, CancellationToken ct)
	{
		Dictionary<Guid, int> shiftsCountForUser = new Dictionary<Guid, int>();

		await foreach (var shift in Database.Shifts.Where(s => s.Type.Season.Id == req.SeasonId).AsAsyncEnumerable()
			               .WithCancellation(ct))
		{
			shiftsCountForUser.TryAdd(shift.EmployeeKeycloakId, 0);
			shiftsCountForUser[shift.EmployeeKeycloakId] += 1;
		}

		var users = await _keycloakService.GetUsers();

		var joined = shiftsCountForUser
			.Join(users, o => o.Key, u => u.Id,
				(o, u) => new KeyValuePair<GetEmployeeResponse, int>(u, o.Value))
			.OrderBy(x => x.Key.FullName.ToLowerInvariant())
			.ToDictionary();

		Response = new ExportAllWhoHaveAShiftResponse
		{
			TotalCount = joined.Count,
			AllEmailAddresses = string.Join(';', joined.Select(x => x.Key.Email)),
			NameAndCount = string.Join('\n',joined.Select(x => $"{x.Key.FullName};{x.Key.Email};{x.Value}"))
		};
	}
}

public class ExportAllWhoHaveAShiftRequest
{
	public Guid SeasonId { get; set; }
}

public class ExportAllWhoHaveAShiftResponse
{
	public string AllEmailAddresses { get; set; }
	public string NameAndCount { get; set; }
	public int TotalCount { get; set; }
}