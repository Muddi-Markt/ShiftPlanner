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
	
	private readonly IReadOnlyCollection<DateTime> _startTimes;
	
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