namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Responses;

public class DefaultEnumerableResponse<TResponse>
{
	public ICollection<TResponse> Data { get; set; }
}
