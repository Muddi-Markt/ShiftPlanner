namespace Muddi.ShiftPlanner.Server.Database.Entities;

public class ShiftContainer
{
	public Guid Id { get; set; }
	public DateTime	Start { get; set; }
	public int TotalShifts { get; set; }
	public ShiftFramework ShiftFramework { get; set; }
	
}