namespace Muddi.ShiftPlanner.Server.Api.Contracts.Responses;

public class DefaultEnumerableResponse<TResponse>
{
	public IEnumerable<TResponse> Data { get; set; }
}
