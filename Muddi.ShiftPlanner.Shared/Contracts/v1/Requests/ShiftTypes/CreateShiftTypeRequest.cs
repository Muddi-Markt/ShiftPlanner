namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Requests;


public class UpdateShiftTypeRequest : CreateShiftTypeRequest
{
	public Guid Id { get; set; }
}

public class CreateShiftTypeRequest
{
	public string Name { get; set; }
	public string Color { get; set; }
	public TimeSpan StartingTimeShift { get; set; }
	public bool OnlyAssignableByAdmin { get; set; }
}