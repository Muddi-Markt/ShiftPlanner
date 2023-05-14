using System.Reflection.Metadata.Ecma335;

namespace Muddi.ShiftPlanner.Server.Database.Entities;

public class ShiftTypeEntity
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string Color { get; set; }
	public TimeSpan StartingTimeShift { get; set; }
	public bool OnlyAssignableByAdmin { get; set; }
	public SeasonEntity Season { get; set; }
	public string? Description { get; set; }
}