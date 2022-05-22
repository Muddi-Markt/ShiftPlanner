using System.ComponentModel.DataAnnotations.Schema;

namespace Muddi.ShiftPlanner.Server.Database.Entities;

public class ShiftContainerEntity
{
	public Guid Id { get; set; }
	public DateTime	Start { get; set; }
	public DateTime End { get; set; }
	public int TotalShifts { get; set; }
	public string Color { get; set; }
	public ShiftLocationEntity Location { get; set; }
	public ShiftFrameworkEntity Framework { get; set; }
	public ICollection<ShiftEntity> Shifts { get; set; }

}