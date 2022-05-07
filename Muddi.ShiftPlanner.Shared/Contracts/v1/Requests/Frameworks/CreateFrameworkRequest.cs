namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Requests;

public class CreateFrameworkRequest
{
	public string Name { get; set; }
	public int SecondsPerShift { get; set; }
	public IList<ShiftFrameworkTypeCountDto> TypeCounts { get; set; }
}

public class ShiftFrameworkTypeCountDto
{
	public Guid ShiftTypeId { get; set; }
	public int Count { get; set; }
}