namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Responses;

public class GetShiftResponse : IMuddiResponse
{
	public Guid Id { get; set; }
	public Guid ContainerId { get; set; }
	public Guid LocationId { get; set; }
	public GetEmployeeResponse? Employee { get; set; }
	public Guid EmployeeId { get; set; }
	public string? EmployeeFullName { get; set; }
	public DateTime Start { get; set; }
	public DateTime End { get; set; }
	public GetShiftTypesResponse? Type { get; set; }
}