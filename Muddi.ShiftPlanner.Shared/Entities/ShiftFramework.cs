namespace Muddi.ShiftPlanner.Shared.Entities;

public class ShiftFramework
{
	public ShiftFramework(TimeSpan timePerShift, Dictionary<ShiftRole, int> rolesCount)
	{
		TimePerShift = timePerShift;
		_rolesCount = rolesCount;
	}

	public TimeSpan TimePerShift { get; }
	private readonly IReadOnlyDictionary<ShiftRole, int> _rolesCount;

	internal int GetCountForRole(ShiftRole role) => _rolesCount[role];
}