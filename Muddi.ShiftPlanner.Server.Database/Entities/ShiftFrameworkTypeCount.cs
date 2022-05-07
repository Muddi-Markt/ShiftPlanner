namespace Muddi.ShiftPlanner.Server.Database.Entities;

public class ShiftFrameworkTypeCount
{
	public Guid Id { get; set; }
	public ShiftFramework ShiftFramework { get; set; }
	public ShiftType ShiftType { get; set; }
	public int Count { get; set; }
}