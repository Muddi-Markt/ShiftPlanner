namespace Muddi.ShiftPlanner.Server.Database.Entities;

public class ShiftFramework
{
	public Guid Id { get; set; }
	public int TimePerShift { get; set; }
	public List<ShiftFrameworkTypeCount> ShiftTypeCounts { get; set; }
}