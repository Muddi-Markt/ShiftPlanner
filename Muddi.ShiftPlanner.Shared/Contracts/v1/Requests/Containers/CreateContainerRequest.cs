namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Requests;

public class CreateContainerRequest
{
	public Guid FrameworkId { get; set; }
	public Guid LocationId { get; set; }
	public DateTime Start { get; set; }
	public int TotalShifts { get; set; }
	public string Color { get; set; }
}