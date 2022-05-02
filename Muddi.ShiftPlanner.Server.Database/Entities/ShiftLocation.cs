namespace Muddi.ShiftPlanner.Server.Database.Entities;

public class ShiftLocation
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public ShiftLocationType Type { get; set; }
	public IList<ShiftContainer> Containers { get; set; }
}