namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Responses;

public class LoginResponse
{
	public string AccessToken { get; set; }
	public DateTime ExpiresAt { get; set; }
}