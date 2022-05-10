using System.Collections.Immutable;
using System.Reflection;
using Muddi.ShiftPlanner.Shared.Exceptions;

namespace Muddi.ShiftPlanner.Shared.Entities;

public class ShiftContainer
{
	public Guid Id { get; }
	public DateTime StartTime { get; }
	public DateTime EndTime { get; }
	public TimeSpan TotalTime { get; }
	public int TotalShifts { get; }
	public ShiftFramework Framework { get; }
	public IEnumerable<DateTime> ShiftStartTimes => _shifts.Keys;

	public ShiftContainer(Guid id, ShiftFramework framework, DateTime startTime, int totalShifts)
	{
		StartTime = startTime.ThrowIfNotUtc();
		EndTime = startTime + framework.TimePerShift * totalShifts;
		TotalTime = EndTime - StartTime;
		TotalShifts = totalShifts;
		Id = id;
		Framework = framework;
		_shifts = StartTimes()
			.ToImmutableSortedDictionary(v => v, k => (ICollection<Shift>)new List<Shift>());

		IEnumerable<DateTime> StartTimes()
		{
			var current = StartTime;
			for (var i = 0; i < TotalShifts; i++)
			{
				yield return current;
				current = current.Add(Framework.TimePerShift);
			}
		}
	}


	private readonly IImmutableDictionary<DateTime, ICollection<Shift>> _shifts;


	internal void AddShift(Shift shift)
	{
		if (!_shifts.TryGetValue(shift.StartTime, out var collection))
			throw new StartTimeNotInContainerException(shift.StartTime);
		if (collection.Any(t => t.StartTime == shift.StartTime
		                        && t.User == shift.User))
			throw new UserAlreadyAssignedException(shift);
		if (collection.Count(t => t.Type == shift.Type) >= Framework.GetCountForRole(shift.Type))
			throw new TooManyWorkersException(shift);

		collection.Add(shift);
	}

	internal void RemoveShift(Shift shift)
	{
		_shifts.Single(t => t.Value.Contains(shift)).Value.Remove(shift);
	}

	public IReadOnlyCollection<Shift> GetShiftsAtGivenTime(DateTime startTime)
	{
		return _shifts[startTime].ToImmutableArray();
	}

	public IEnumerable<ShiftType> GetAvailableRolesAtGivenTime(DateTime startTime)
	{
		var rolesCount = new Dictionary<ShiftType, int>(Framework.RolesCount);
		foreach (var shift in _shifts[startTime])
		{
			rolesCount[shift.Type]--;
		}

		return rolesCount.Where(t => t.Value > 0).Select(t => t.Key);
	}


	public IEnumerable<Shift> GetAllShifts() => _shifts.SelectMany(t => t.Value);
	public bool IsTimeWithinContainer(DateTime time) => time >= StartTime && time < EndTime;

	public DateTime GetBestShiftStartTimeForTime(DateTime startTime)
	{
		startTime -= Framework.TimePerShift;
		DateTime last = default;
		foreach (var shiftStartTime in ShiftStartTimes)
		{
			last = shiftStartTime;
			if (shiftStartTime > startTime)
				return shiftStartTime;
		}

		return last;
	}
}