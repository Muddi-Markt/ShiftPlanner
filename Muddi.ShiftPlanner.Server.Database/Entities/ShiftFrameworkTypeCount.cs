namespace Muddi.ShiftPlanner.Server.Database.Entities;

public class ShiftFrameworkTypeCount
{
	public Guid Id { get; set; }
	public Guid ShiftFrameworkId { get; set; }
	public Guid ShiftTypeId { get; set; }
	public int Count { get; set; }
}