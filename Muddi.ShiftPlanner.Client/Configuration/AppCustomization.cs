using System.Text.Json.Serialization;
using Muddi.ShiftPlanner.Shared.Json;

public class AppCustomization
{
	public string Title { get; init; } = "Schichtplanner";
	public string Subtitle { get; init; } = string.Empty;
	public string Contact { get; init; } = "add@me.de";
	public string ContactHref => (Contact.Contains('@') ? "mailto:" + Contact : "#");
	[JsonConverter(typeof(TimeSpanHourFormatConverter))]
	public TimeSpan StartTime { get; init; } = TimeSpan.FromHours(8); //08:00:00
	public TimeSpan EndTime { get; init; } = TimeSpan.FromHours(26); //26:00:00
}