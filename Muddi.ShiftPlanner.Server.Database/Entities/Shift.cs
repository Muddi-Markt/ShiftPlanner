namespace Muddi.ShiftPlanner.Server.Database.Entities;

public class Shift
{
	public Guid Id { get; set; }
	public Guid EmployeeKeycloakId { get; set; }
	public DateTime Start { get; set; }
	public DateTime End { get; set; }
	public ShiftType Type { get; set; }
	
}