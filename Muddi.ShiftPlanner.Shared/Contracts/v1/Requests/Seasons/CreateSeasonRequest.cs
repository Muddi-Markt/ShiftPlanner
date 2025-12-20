namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Seasons;

public class CreateSeasonRequest
{
	public string Name { get; set; }
	public DateTime StartDate { get; set; }
	public DateTime EndDate { get; set; }

	/// <summary>
	/// Optional: Copy ShiftTypes, Frameworks, Locations, and Containers from an existing season.
	/// Container dates will be offset based on the difference between source and target season start dates.
	/// </summary>
	public Guid? CopyFromSeasonId { get; set; }
}