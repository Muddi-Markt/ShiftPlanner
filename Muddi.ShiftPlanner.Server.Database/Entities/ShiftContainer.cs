using System.ComponentModel.DataAnnotations.Schema;

namespace Muddi.ShiftPlanner.Server.Database.Entities;

public class ShiftContainer
{
	public Guid Id { get; set; }
	public DateTime	Start { get; set; }
	public DateTime End { get; set; }
	public int TotalShifts { get; set; }
	public ShiftFramework Framework { get; set; }
	public ICollection<Shift> Shifts { get; set; }

}