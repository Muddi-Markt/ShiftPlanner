using System.Diagnostics;
using System.Text.RegularExpressions;
using Muddi.ShiftPlanner.Client.Entities;
using Muddi.ShiftPlanner.Client.Services;
using Muddi.ShiftPlanner.Shared;
using Muddi.ShiftPlanner.Shared.Entities;
using Radzen;
using Radzen.Blazor.Rendering;
using Appointment = Muddi.ShiftPlanner.Client.Entities.Appointment;

namespace Muddi.ShiftPlanner.Client;

public static class RenderExtensions
{
	public static void SetSlotRenderStyle(this SchedulerSlotRenderEventArgs args,
		IReadOnlyList<ShiftContainer> containers)
	{
		switch (args.View.Text)
		{
			// Highlight today in month view
			case "Monat" when args.Start.Date == DateTime.Today:
				args.Attributes["style"] = "background: rgba(255,220,40,.2);";
				break;
			// Highlight shift hours
			case "Woche" or "Tag":
			{
				foreach (var container in containers)
				{
					var c = container.BackgroundColor;
					if (args.Start >= container.StartTime.ToLocalTime() && args.Start < container.EndTime.ToLocalTime())
						args.Attributes["style"] = $"background: {c};";
				}

				break;
			}
		}
	}

	public static void SetAppointmentRenderStyle(this SchedulerAppointmentRenderEventArgs<Appointment> args,
		Guid userKeycloakId)
	{
		args.Attributes["style"] = string.Empty;

		if (args.Data is WeekAppointment weekAppointment)
		{
			var from = weekAppointment.AvailableShifts;
			var to = weekAppointment.TotalShifts;
			double num = Convert.ToInt32(40 + 40 * from / to) / 100.0;
			args.Attributes["style"] = $"background: rgb(0,0,0,{num.ToInvariantString()});";
			return;
		}

		if (args.Data is not DayAppointment dayAppointment)
			throw new NotSupportedException("Unknown appointment type" + args.Data.GetType());
		var shift = dayAppointment.Shift;

		//DayView
		if (shift.User == Mappers.NotAssignedEmployee)
		{
			args.Attributes["style"] += $"background: #ffffffD0; color:{shift.Type.Color};";
			return;
		}

		args.Attributes["style"] += $"background: {shift.Type.Color};";
		if (shift.User.KeycloakId == userKeycloakId)
		{
			args.Attributes["style"] += "outline: 2px dashed #852121;";
		}
	}
}