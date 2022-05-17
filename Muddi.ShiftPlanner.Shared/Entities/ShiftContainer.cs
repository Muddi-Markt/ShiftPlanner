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
	public IEnumerable<DateTime> ShiftStartTimes => _startTimes;
	public string BackgroundColor { get; set; }

	public ShiftContainer(Guid id, ShiftFramework framework, DateTime startTime, int totalShifts, string color)
	{
		BackgroundColor = color;
		StartTime = startTime.ThrowIfNotUtc();
		EndTime = startTime + framework.TimePerShift * totalShifts;
		TotalTime = EndTime - StartTime;
		TotalShifts = totalShifts;
		Id = id;
		Framework = framework;
		_startTimes = StartTimes().ToImmutableList();
		_shifts = new List<Shift>();

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


	private readonly IList<Shift> _shifts;
	private readonly IReadOnlyCollection<DateTime> _startTimes;


	internal void AddShift(Shift shift)
	{
		if (!_startTimes.Contains(shift.StartTime))
			throw new StartTimeNotInContainerException(shift.StartTime);
		if (_shifts.Any(t => t.StartTime == shift.StartTime
		                        && t.User == shift.User))
			throw new UserAlreadyAssignedException(shift);
		if (_shifts.Count(t => t.Type == shift.Type) >= Framework.GetCountForRole(shift.Type))
			throw new TooManyWorkersException(shift);

		_shifts.Add(shift);
	}

	internal void RemoveShift(Shift shift)
	{
		_shifts.Remove(shift);
	}

	public IEnumerable<Shift> GetShiftsAtGivenTime(DateTime startTime)
	{
		return _shifts.Where(s => s.StartTime == startTime);
	}

	public IEnumerable<ShiftType> GetAvailableRolesAtGivenTime(DateTime startTime)
	{
		var rolesCount = new Dictionary<ShiftType, int>(Framework.RolesCount);
		foreach (var shift in GetShiftsAtGivenTime(startTime))
		{
			rolesCount[shift.Type]--;
		}

		return rolesCount.Where(t => t.Value > 0).Select(t => t.Key);
	}


	public IEnumerable<Shift> GetAllShifts() => _shifts;
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