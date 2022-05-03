namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Requests.Containers;

public class CreateContainerRequest
{
	public Guid FrameworkId { get; set; }
	public DateTime Start { get; set; }
	public int TotalShifts { get; set; }
}