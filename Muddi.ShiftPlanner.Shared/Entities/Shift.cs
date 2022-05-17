namespace Muddi.ShiftPlanner.Shared.Entities;

public class Shift : IEquatable<Shift>
{
	internal Shift(Guid id, EmployeeBase user, DateTime startTime, DateTime endTime, ShiftType type, Guid containerId)
	{
		User = user;
		StartTime = startTime;
		EndTime = endTime;
		Type = type;
		ContainerId = containerId;
		Id = id;
	}
	
	public Shift(EmployeeBase user, DateTime startTime, DateTime endTime, ShiftType type)
	{
		User = user;
		StartTime = startTime;
		EndTime = endTime;
		Type = type;
	}

	public string Title => (User.Name.Contains(' ') ? User.Name[..User.Name.IndexOf(' ')] : User.Name) + "\n" + Type.Name;
	public EmployeeBase User { get; }
	public DateTime StartTime { get; }
	public DateTime DisplayStartTime => StartTime.ToLocalTime() + Type.StartingTimeShift;
	public DateTime EndTime { get; }
	public DateTime DisplayEndTime => EndTime.ToLocalTime() + Type.StartingTimeShift;
	public ShiftType Type { get; }
	public Guid ContainerId { get; }
	public Guid Id { get; }

	public bool Equals(Shift? other)
	{
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;
		return User.Equals(other.User) && StartTime.Equals(other.StartTime) && EndTime.Equals(other.EndTime) && Type.Equals(other.Type);
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
		return HashCode.Combine(User, StartTime, EndTime, Type);
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