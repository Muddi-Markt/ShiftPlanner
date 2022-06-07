using System.Text;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Muddi.ShiftPlanner.Server.Database.Entities;

namespace Muddi.ShiftPlanner.Server.Api.Extensions;

public static class CalendarExtensions
{
	public static Calendar ToICalCalendar(this IEnumerable<ShiftEntity> shifts)
	{
		var events = shifts.Select(s => new CalendarEvent
		{
			Start = new CalDateTime(s.Start + s.Type.StartingTimeShift),
			End = new CalDateTime(s.End + s.Type.StartingTimeShift),
			Uid = s.Id.ToString(),
			Location = "Muddi Markt e.V. Kiel",
			GeographicLocation = new GeographicLocation(54.32557369383779, 10.134938948950177),
			Summary = $"{s.Type.Name} Schicht@{s.ShiftContainer.Location.Name}"
		});
		var calendar = new Calendar() { ProductId = "https://github.com/Muddi-Markt/ShiftPlanner//NONSGML ical.net 4.0//EN" };
		calendar.Events.AddRange(events);
		return calendar;
	}

	public static byte[] ToByteArray(this Calendar calendar)
	{
		var serializer = new CalendarSerializer();
		var str = serializer.SerializeToString(calendar);
		return Encoding.UTF8.GetBytes(str);
	}
}