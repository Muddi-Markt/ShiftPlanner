namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Responses;

public class GetFrameworkResponse : IMuddiResponse
{
	private IEnumerable<ShiftFrameworkTypeCountResponse> _shiftTypeCounts = [];
	public Guid Id { get; set; }
	public string Name { get; set; }
	public int SecondsPerShift { get; set; }

	public IEnumerable<ShiftFrameworkTypeCountResponse> ShiftTypeCounts
	{
		get => _shiftTypeCounts;
		set => _shiftTypeCounts = value.OrderBy(x => x.ShiftType.Name);
	}
}