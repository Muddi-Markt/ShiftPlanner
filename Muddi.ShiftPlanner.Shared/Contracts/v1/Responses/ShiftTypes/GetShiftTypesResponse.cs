namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Responses;

public class GetShiftTypesResponse : IMuddiResponse
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string Color { get; set; }
	public TimeSpan StartingTimeShift { get; set; }
	public bool OnlyAssignableByAdmin { get; set; }
	public string? Description { get; set; }
}