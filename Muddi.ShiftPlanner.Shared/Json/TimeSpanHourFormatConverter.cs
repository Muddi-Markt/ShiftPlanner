using System.Text.Json;
using System.Text.Json.Serialization;

namespace Muddi.ShiftPlanner.Shared.Json;

public class TimeSpanHourFormatConverter : JsonConverter<TimeSpan>
{
	public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var timeString = reader.GetString();
		if (timeString == null)
			throw new JsonException("TimeSpan string is null");
			
		var parts = timeString.Split(':');
		if (parts.Length is 2 or 3 && int.TryParse(parts[0], out var hours) && int.TryParse(parts[1], out var minutes))
		{
			var seconds = parts.Length == 3 && int.TryParse(parts[2], out var s) ? s : 0;
			return new TimeSpan(hours, minutes, seconds);
		}
	
		throw new JsonException($"Unable to parse TimeSpan: {timeString}");
	}
	
	public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
	{
		writer.WriteStringValue($"{(int)value.TotalHours}:{value.Minutes:00}:{value.Seconds:00}");
	}
}