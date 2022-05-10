namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Responses;

public class GetEmployeeResponse
{
	public Guid Id { get; set; }
	public string UserName { get; set; }
	public string Email { get; set; }
	public string? FirstName { get; set; }
	public string? LastName { get; set; }
}