namespace Muddi.ShiftPlanner.Shared.Entities;

public record ShiftRole(Guid Id, string Name);

public enum UserRoles
{
	Viewer,
	Worker,
	Manager,
	Admin
}

public abstract class WorkingUserBase : IEquatable<WorkingUserBase>
{
	public WorkingUserBase(Guid keycloakId, string name)
	{
		UserRole = UserRoles.Worker; //TODO make roles with keycloak
		KeycloakId = keycloakId;
		Name = name;
	}

	public Guid KeycloakId { get; }
	public string Name { get; }

	public UserRoles UserRole { get; }


	public bool Equals(WorkingUserBase? other)
	{
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;
		return KeycloakId.Equals(other.KeycloakId);
	}

	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj.GetType() != GetType()) return false;
		return Equals((WorkingUserBase)obj);
	}

	public override int GetHashCode()
	{
		return KeycloakId.GetHashCode();
	}

	public static bool operator ==(WorkingUserBase? left, WorkingUserBase? right)
	{
		return Equals(left, right);
	}

	public static bool operator !=(WorkingUserBase? left, WorkingUserBase? right)
	{
		return !Equals(left, right);
	}

	public override string ToString() => $"{Name} ({KeycloakId})";
}