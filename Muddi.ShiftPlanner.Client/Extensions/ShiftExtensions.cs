﻿using Muddi.ShiftPlanner.Client.Entities;
using Muddi.ShiftPlanner.Shared.Entities;

namespace Muddi.ShiftPlanner.Client;

public static class ShiftExtensions
{
	public static Appointment ToAppointment(this Shift shift) => new(shift);
}