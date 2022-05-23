using Muddi.ShiftPlanner.Client.Entities;
using Muddi.ShiftPlanner.Client.Services;
using Muddi.ShiftPlanner.Shared.Entities;
using Radzen;

namespace Muddi.ShiftPlanner.Client;

public static class RenderExtensions
{

	public static void SetSlotRenderStyle(this SchedulerSlotRenderEventArgs args, IReadOnlyList<ShiftContainer> containers)
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
	
	public static void SetAppointmentRenderStyle(this SchedulerAppointmentRenderEventArgs<Appointment> args, Guid userKeycloakId)
	{
		args.Attributes["style"] = string.Empty;
		if (args.Data.Shift is not { } shift)
		{
			if (args.Data.Title.StartsWith('0'))
				args.Attributes["style"] += $"background: #00000033";
			else
				args.Attributes["style"] += $"background: #000000BB";
			return;
		}

		if (shift.User == ShiftService.NotAssignedEmployee)
		{
			args.Attributes["style"] += $"background: #555; color:{shift.Type.Color};";
			return;
		}

		args.Attributes["style"] += $"background: {shift.Type.Color};";
		if (shift.User.KeycloakId == userKeycloakId)
		{
			args.Attributes["style"] += "border: 2px dashed #852121;";
		}
	}
}