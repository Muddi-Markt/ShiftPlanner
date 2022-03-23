using System.Collections.Immutable;
using System.Reflection;
using Muddi.ShiftPlanner.Shared.Exceptions;

namespace Muddi.ShiftPlanner.Shared.Entities;

public class ShiftContainer
{
	public DateTime StartTime { get; }
	public DateTime EndTime { get; }
	public TimeSpan TotalTime { get; }
	public int TotalShifts { get; }
	private readonly ShiftFramework _framework;

	public ShiftContainer(ShiftFramework framework, DateTime startTime, int totalShifts)
	{
		StartTime = startTime.ThrowIfNotUtc();
		EndTime = startTime + framework.TimePerShift * totalShifts;
		TotalTime = EndTime - StartTime;
		TotalShifts = totalShifts;
		_framework = framework;
		_shifts = GetStartTimes()
			.ToImmutableDictionary(v => v, k => (ICollection<Shift>)new List<Shift>());
	}


	private readonly IImmutableDictionary<DateTime, ICollection<Shift>> _shifts;

	internal void AddShift(Shift shift)
	{
		if (!_shifts.TryGetValue(shift.StartTime, out var collection))
			throw new KeyNotFoundException($"{shift.StartTime} is no valid start date for this shift");
		if (collection.Count(t => t.Role == shift.Role) >= _framework.GetCountForRole(shift.Role))
			throw new TooManyWorkersException(shift);
		if (collection.Any(t => t == shift))
			throw new AmbiguousMatchException("A shift for the same time and for the same user is already set");
		collection.Add(shift);
	}

	internal void RemoveShift(Shift shift)
	{
		_shifts.Single(t => t.Value.Contains(shift)).Value.Remove(shift);
	}

	public IReadOnlyCollection<Shift> GetShiftsAtGivenTime(DateTime date)
	{
		return _shifts[date].ToImmutableArray();
	}

	private IEnumerable<DateTime> GetStartTimes()
	{
		var current = StartTime;
		for (var i = 0; i < TotalShifts; i++)
		{
			yield return current;
			current = current.Add(_framework.TimePerShift);
		}
	}

	public IEnumerable<Shift> GetAllShifts()
	{
		return _shifts.SelectMany(t => t.Value);
	}

	public bool IsShiftInContainer(Shift shift)
	{
		return shift.StartTime >= StartTime && shift.StartTime < EndTime;
	}
}