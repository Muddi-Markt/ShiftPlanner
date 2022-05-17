namespace Muddi.ShiftPlanner.Server.Database.Entities;

public class ShiftFrameworkTypeCountEntity
{
	public Guid Id { get; set; }
	public ShiftFrameworkEntity ShiftFramework { get; set; }
	public ShiftTypeEntity ShiftType { get; set; }
	public int Count { get; set; }
}