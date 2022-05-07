using System.ComponentModel.DataAnnotations.Schema;

namespace Muddi.ShiftPlanner.Server.Database.Entities;

public class ShiftFramework
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public int SecondsPerShift { get; set; }
	public List<ShiftFrameworkTypeCount> ShiftTypeCounts { get; set; }
	[NotMapped] public TimeSpan TimePerShift => TimeSpan.FromSeconds(SecondsPerShift);
	
}