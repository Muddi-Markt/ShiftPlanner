using Muddi.ShiftPlanner.Client.Entities;
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
					var maxTimeShift = container.Framework.RolesCount.Select(x => x.Key.StartingTimeShift).Max();

					if (args.Start >= container.StartTime.ToLocalTime() &&
					    args.Start < container.EndTime.Add(maxTimeShift).ToLocalTime())
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
		// Blocked shifts - check before free shifts, since blocked shifts also have NotAssignedEmployee as user
		if (shift.IsBlocked)
		{
			var color = !string.IsNullOrEmpty(shift.Type.Color)
				? shift.Type.Color
				: "#666";
			args.Attributes["style"] += $"""
			                             color: #666;
			                             border: 1px solid {color};
			                             background: repeating-linear-gradient(
			                               45deg,
			                               #ebebeb,       /* Stripe 1 color */
			                               #ebebeb 15px,  /* Stripe 1 width */
			                               #cdcdcd 15px,  /* Stripe 2 color starts immediately */
			                               #cdcdcd 30px   /* Stripe 2 width ends */
			                             );
			                             """;
			args.Attributes["title"] = "Gesperrt: " + shift.BlockReason;
			return;
		}

		// Free shifts - plain white
		if (shift.User == Mappers.NotAssignedEmployee)
		{
			var textColor = !string.IsNullOrEmpty(shift.Type.Color)
				? shift.Type.Color
				: "#333333";
			args.Attributes["style"] += $"background: #ffffff; color:{textColor}; border: 1px solid {textColor};";
			return;
		}

		var normalBg = !string.IsNullOrEmpty(shift.Type.Color) ? shift.Type.Color : "#cccccc";
		args.Attributes["style"] += $"background: {normalBg};";
		if (shift.User.KeycloakId == userKeycloakId)
		{
			args.Attributes["style"] += "outline: 2px dashed #852121;";
		}
	}
}