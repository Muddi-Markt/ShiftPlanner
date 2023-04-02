using System.Reflection.Emit;

namespace Muddi.ShiftPlanner.Server.Database.Entities;

public class SeasonEntity
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public DateTime StartDate { get; set; }
	public DateTime EndDate { get; set; }
}