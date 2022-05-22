namespace Muddi.ShiftPlanner.Server.Database.Entities;

public class ShiftEntity
{
	public Guid Id { get; set; }
	public Guid EmployeeKeycloakId { get; set; }
	public ShiftContainerEntity ShiftContainer { get; set; }
	public DateTime Start { get; set; }
	public DateTime End { get; set; }
	public ShiftTypeEntity Type { get; set; }
	
}