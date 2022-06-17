namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Requests;

public class ExportToExcelRequest
{
	public Guid LocationId { get; set; }
	public bool Anonymous { get; set; } = false;
}