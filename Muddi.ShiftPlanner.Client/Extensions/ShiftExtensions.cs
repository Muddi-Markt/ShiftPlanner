using Muddi.ShiftPlanner.Client.Entities;
using Muddi.ShiftPlanner.Shared.Entities;

namespace Muddi.ShiftPlanner.Client;

public static class ShiftExtensions
{
	public static DayAppointment ToAppointment(this Shift shift) => new(shift);
}