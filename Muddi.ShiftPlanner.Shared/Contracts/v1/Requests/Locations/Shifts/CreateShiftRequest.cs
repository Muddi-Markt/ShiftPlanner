namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Requests;

public class CreateShiftRequest
{
	public Guid Id { get; set; }
	public Guid EmployeeKeycloakId { get; set; }
	public DateTime Start { get; set; }
	public Guid ShiftTypeId { get; set; }
}