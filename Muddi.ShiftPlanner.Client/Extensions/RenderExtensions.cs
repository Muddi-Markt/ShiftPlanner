using System.Text.RegularExpressions;
using Muddi.ShiftPlanner.Client.Entities;
using Muddi.ShiftPlanner.Client.Services;
using Muddi.ShiftPlanner.Shared;
using Muddi.ShiftPlanner.Shared.Entities;
using Radzen;

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

		//WeekView (quite hacky ;) )
		if (args.Data.Shift is not { } shift)
		{
			var match = Regex.Match(args.Data.Title, @"(\d*?)\/(\d*?)\n");
			var from = double.Parse(match.Groups[1].Value);
			var to = double.Parse(match.Groups[2].Value);
			int num = Convert.ToInt32(0x33 + (136.0 * from / to));
			var numHex = num.ToString("X2");

			args.Attributes["style"] += $"background: #000000{numHex}";
			return;
		}


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