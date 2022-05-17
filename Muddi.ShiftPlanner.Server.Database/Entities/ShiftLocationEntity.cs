namespace Muddi.ShiftPlanner.Server.Database.Entities;

public class ShiftLocationEntity
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public ShiftLocationTypeEntity Type { get; set; }
	public IList<ShiftContainer> Containers { get; set; }
}