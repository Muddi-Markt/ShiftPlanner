using Muddi.ShiftPlanner.Shared.Entities;

namespace Muddi.ShiftPlanner.Client.Entities;

public class Appointment
{
	public Appointment(Shift shift)
		: this(shift.StartTime, shift.EndTime)
	{
		Shift = shift;
		Title = StartTimeWithTimeShift.ToString("HH:mm") + " - " + EndTimeWithTimeShift.ToString("HH:mm") + "\n" 
		        + shift.User.Name 
		        + "\n" + shift.Type.Name;
	}

	public Appointment(DateTime startTime, DateTime endTime, string title)
		: this(startTime, endTime)
	{
		Title = title;
	}

	private Appointment(DateTime startTime, DateTime endTime)
	{
		LocalStartTime = startTime.ToLocalTime();
		LocalEndTime = endTime.ToLocalTime();
		Title = string.Empty;
	}

	public DateTime LocalStartTime { get; init; }
	public DateTime LocalEndTime { get; init; }
	public string Title { get; init; }
	public Shift? Shift { get; init; }
	public DateTime StartTimeWithTimeShift => LocalStartTime + (Shift?.Type.StartingTimeShift ?? TimeSpan.Zero);
	public DateTime EndTimeWithTimeShift => LocalEndTime + (Shift?.Type.StartingTimeShift ?? TimeSpan.Zero);
}