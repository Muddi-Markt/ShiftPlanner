using System.Collections.Immutable;

namespace Muddi.ShiftPlanner.Shared.Entities;

public class ShiftFramework
{
	public ShiftFramework(TimeSpan timePerShift, Dictionary<ShiftRole, int> rolesCount)
	{
		TimePerShift = timePerShift;
		RolesCount = rolesCount.ToImmutableDictionary();
	}

	public TimeSpan TimePerShift { get; }
	public IImmutableDictionary<ShiftRole, int> RolesCount;

	internal int GetCountForRole(ShiftRole role) => RolesCount[role];
}