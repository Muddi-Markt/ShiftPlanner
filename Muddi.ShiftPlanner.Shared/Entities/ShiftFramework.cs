using System.Collections.Immutable;

namespace Muddi.ShiftPlanner.Shared.Entities;

public class ShiftFramework
{
	public ShiftFramework(Guid id,TimeSpan timePerShift, Dictionary<ShiftType, int> rolesCount)
	{
		TimePerShift = timePerShift;
		Id = id;
		RolesCount = rolesCount.ToImmutableDictionary();
	}
	public Guid Id { get; }

	public TimeSpan TimePerShift { get; }
	public IImmutableDictionary<ShiftType, int> RolesCount;

	internal int GetCountForRole(ShiftType type) => RolesCount[type];
}