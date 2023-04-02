namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Requests;

public class UpdateLocationRequest : CreateLocationRequest, IUpdateRequest
{
	public Guid Id { get; set; }
}

public interface IUpdateRequest
{
	Guid Id { get; set; }
}

public class CreateLocationRequest
{
	public string Name { get; set; }
	public Guid TypeId { get; set; }
	public Guid SeasonId { get; set; }
}