namespace Muddi.ShiftPlanner.Server.Api.Endpoints.Seasons;

public class UpdateSeasonRequest : CreateSeasonRequest
{
	public Guid Id { get; set; }
}