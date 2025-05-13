using System.Text.Json;

namespace Muddi.ShiftPlanner.Client.Configuration;

public class AppCustomization
{
	public string Title { get; init; } = "Schichtplanner";
	public string Subtitle { get; init; } = string.Empty;
	public string Contact { get; init; } = "add@me.de";
	public string ContactHref => (Contact.Contains('@') ? "mailto:" + Contact : "#");
	public string StartTime { get; init; } = "08:00:00";
	public string EndTime { get; init; } = "26:00:00";

	//We have to Parse the time span every time because the configuration mapping uses some kind of reflection
	//that will not call the official init; or set; properties of StartTime/EndTime property
	public TimeSpan StartTimeSpan => ParseTimeSpan(StartTime);
	public TimeSpan EndTimeSpan => ParseTimeSpan(EndTime);

	private static TimeSpan ParseTimeSpan(string timeString)
	{
		if (timeString.Contains('.')) //e.g. 1.02:00:00
			return TimeSpan.Parse(timeString);
		var parts = timeString.Split(':');
		if (parts.Length is 2 or 3 && int.TryParse(parts[0], out var hours) && int.TryParse(parts[1], out var minutes))
		{
			var seconds = parts.Length == 3 && int.TryParse(parts[2], out var s) ? s : 0;
			return new TimeSpan(hours, minutes, seconds);
		}

		throw new JsonException($"Unable to parse TimeSpan: {timeString}");
	}
}