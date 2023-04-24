namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Responses.Seasons;

public class GetSeasonResponse : IMuddiResponse
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public DateTime StartDate { get; set; }
	public DateTime EndDate { get; set; }
	public bool IsSelectedAsCurrent { get; set; }
}