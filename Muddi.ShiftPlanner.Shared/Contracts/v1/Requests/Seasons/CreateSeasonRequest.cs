namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Seasons;

public class CreateSeasonRequest
{
	public string Name { get; set; }
	public DateTime StartDate { get; set; }
	public DateTime EndDate { get; set; }
}