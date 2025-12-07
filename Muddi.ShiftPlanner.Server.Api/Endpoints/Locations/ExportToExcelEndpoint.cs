using System.Web;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Api.Extensions;
using Muddi.ShiftPlanner.Server.Api.Services;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Shared.Contracts.v1;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Locations;

public class ExportToExcelEndpoint : Endpoint<ExportToExcelRequest>
{
	private readonly ShiftPlannerContext _database;
	private readonly ExcelService _excel;

	public override void Configure()
	{
		Roles(ApiRoles.Editor);
		Get("/locations/{LocationId}/export/xlsx");
// #if DEBUG
// 		AllowAnonymous();
// #else
// 		#error Don't AllowAnonymous ever at default
// #endif
	}

	public override async Task HandleAsync(ExportToExcelRequest req, CancellationToken ct)
	{
		var location = await _database.ShiftLocations
			.CheckAdminOnly(User)
			.Include(l => l.Containers)
			.ThenInclude(c => c.Framework)
			.ThenInclude(f => f.ShiftTypeCounts)
			.ThenInclude(t => t.ShiftType)
			.Include(l => l.Containers)
			.ThenInclude(c => c.Shifts)
			.ThenInclude(s => s.Type)
			.AsSingleQuery()
			.AsNoTracking()
			.FirstAsync(l => l.Id == req.LocationId, cancellationToken: ct);

		var bytes = _excel.ExportLocationToXlsx(location, req.Anonymous);


		var fileName = $"{location.Name.Replace(' ', '_').ToLower()}.xlsx";
		fileName = HttpUtility.UrlEncode(fileName);

		await Send.BytesAsync(bytes,
			fileName: fileName,
			contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
			cancellation: ct);
	}


	public ExportToExcelEndpoint(ShiftPlannerContext database, ExcelService excel)
	{
		_database = database;
		_excel = excel;
	}
}