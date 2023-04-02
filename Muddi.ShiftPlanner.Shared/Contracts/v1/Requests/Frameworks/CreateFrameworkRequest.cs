namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Requests;

public class UpdateFrameworkRequest : CreateFrameworkRequest
{
	public Guid Id { get; set; }
}
public class CreateFrameworkRequest
{
	public string Name { get; set; }
	public int SecondsPerShift { get; set; }
	public Guid SeasonId { get; set; }
	public IList<ShiftFrameworkTypeCountDto> TypeCounts { get; set; }
}

public class ShiftFrameworkTypeCountDto
{
	public Guid ShiftTypeId { get; set; }
	public int Count { get; set; }
}