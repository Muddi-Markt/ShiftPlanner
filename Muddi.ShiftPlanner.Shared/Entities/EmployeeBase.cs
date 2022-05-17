namespace Muddi.ShiftPlanner.Shared.Entities;

public record ShiftType(
	Guid Id, 
	string Name, 
	string Color, 
	bool OnlyAssignableByAdmin, 
	TimeSpan StartingTimeShift);

public enum UserRoles
{
	Viewer,
	Worker,
	Manager,
	Admin
}

public class Employee : EmployeeBase
{
	public Employee(Guid keycloakId, string name) : base(keycloakId, name)
	{
	}
}

public abstract class EmployeeBase : IEquatable<EmployeeBase>
{
	public EmployeeBase(Guid keycloakId, string name)
	{
		UserRole = UserRoles.Worker; //TODO make roles with keycloak
		KeycloakId = keycloakId;
		Name = name;
	}

	public Guid KeycloakId { get; }
	public string Name { get; }

	public UserRoles UserRole { get; }


	public bool Equals(EmployeeBase? other)
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
		return Equals((EmployeeBase)obj);
	}

	public override int GetHashCode()
	{
		return KeycloakId.GetHashCode();
	}

	public static bool operator ==(EmployeeBase? left, EmployeeBase? right)
	{
		return Equals(left, right);
	}

	public static bool operator !=(EmployeeBase? left, EmployeeBase? right)
	{
		return !Equals(left, right);
	}

	public override string ToString() => $"{Name} ({KeycloakId})";
}