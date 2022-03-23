namespace Muddi.ShiftPlanner.Shared.Entities;

public class Shift : IEquatable<Shift>
{
	internal Shift(WorkingUserBase user, DateTime startTime, DateTime endTime, ShiftRole role)
	{
		User = user;
		StartTime = startTime;
		EndTime = endTime;
		Role = role;
	}

	public string Title => User.Name;
	public WorkingUserBase User { get; }
	public DateTime StartTime { get; }
	public DateTime EndTime { get; }
	public ShiftRole Role { get; }

	public bool Equals(Shift? other)
	{
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;
		return User.Equals(other.User) && StartTime.Equals(other.StartTime) && EndTime.Equals(other.EndTime) && Role.Equals(other.Role);
	}

	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj.GetType() != GetType()) return false;
		return Equals((Shift)obj);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(User, StartTime, EndTime, Role);
	}

	public static bool operator ==(Shift? left, Shift? right)
	{
		return Equals(left, right);
	}

	public static bool operator !=(Shift? left, Shift? right)
	{
		return !Equals(left, right);
	}
}