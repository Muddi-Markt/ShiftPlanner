namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Responses;

public class GetShiftResponse
{
	public Guid Id { get; set; }
	public Guid EmployeeKeycloakId { get; set; }
	public DateTime Start { get; set; }
	public DateTime End { get; set; }
	public GetShiftTypesResponse Type { get; set; }
}