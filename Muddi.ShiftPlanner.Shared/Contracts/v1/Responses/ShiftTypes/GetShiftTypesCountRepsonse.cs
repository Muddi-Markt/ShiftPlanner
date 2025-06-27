namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Responses;

public class GetShiftTypesCountRequest
{
	public int? Limit { get; set; }
	public bool IncludeNonAvailable { get; set; }
	public DateTime? StartingFrom { get; set; }
	public Guid SeasonId { get; set; }
}
public class GetShiftTypesCountResponse
{
	public GetShiftTypesResponse Type { get; init; }
	public int AvailableCount { get; set; }
	public int TotalCount { get; init; }
	public DateTime Start { get; init; }
	public DateTime End { get; init; }
	public Guid LocationId { get; init; }
}