namespace Muddi.ShiftPlanner.Shared.Entities;

public record ShiftRole(Guid Id, string Name);

public abstract class WorkingUserBase
{
	public WorkingUserBase(Guid keycloakId)
	{
		KeycloakId = keycloakId;
	}

	public Guid KeycloakId { get; }
}