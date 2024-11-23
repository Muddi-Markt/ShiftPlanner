using Muddi.ShiftPlanner.Shared.Entities;

namespace Muddi.ShiftPlanner.Client.Entities;

public class WeekAppointment : Appointment
{
	public int AvailableShifts { get; init; }
	public int TotalShifts { get; init; }

	public WeekAppointment(DateTime startTime, DateTime endTime, string title, int availableShifts, int totalShifts) :
		base(startTime, endTime, title)
	{
		AvailableShifts = availableShifts;
		TotalShifts = totalShifts;
	}
}

public class DayAppointment : Appointment
{
	public Shift Shift { get; init; }

	public DayAppointment(Shift shift) : base(shift.StartTime, shift.EndTime)
	{
		Shift = shift;
		if (shift.Duration < TimeSpan.FromMinutes(60))
			Title = shift.Type.Name;
		else if (shift.Duration < TimeSpan.FromMinutes(90))
			Title = shift.Type.Name + "\n"
			                        + shift.User.Name + "\n";
		else
			Title = shift.Type.Name + "\n"
			                        + shift.User.Name + "\n"
			                        + TimeString;
	}

	private string TimeString
		=> StartTimeWithTimeShift.ToString("HH:mm") + " - " + EndTimeWithTimeShift.ToString("HH:mm");

	public DateTime StartTimeWithTimeShift => LocalStartTime + Shift.Type.StartingTimeShift;
	public DateTime EndTimeWithTimeShift => LocalEndTime + Shift.Type.StartingTimeShift;
}

public abstract class Appointment
{
	protected Appointment(DateTime startTime, DateTime endTime, string title)
		: this(startTime, endTime)
	{
		Title = title;
	}

	protected Appointment(DateTime startTime, DateTime endTime)
	{
		LocalStartTime = startTime.ToLocalTime();
		LocalEndTime = endTime.ToLocalTime();
		Title = string.Empty;
	}

	public DateTime LocalStartTime { get; init; }
	public DateTime LocalEndTime { get; init; }
	public string Title { get; init; }
}