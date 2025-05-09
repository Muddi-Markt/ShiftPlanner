using System.Text.Json;
using FluentAssertions;
using Muddi.ShiftPlanner.Client.Configuration;
using Xunit;

namespace Muddi.ShiftPlanner.Tests.Unit;

public class JsonTests
{
	[Fact]
	public void ShouldDeserializeAppCustomization_WhenValid()
	{
		string json = """
		              {
		                "Title": "MUDDIs Schicht Planner",
		                "Subtitle": "Auf eine HAMMER-mäßige Kieler Woche 2023!",
		                "Contact": "gastro@muddimarkt.org",
		                "StartTime": "08:00:00",
		                "EndTime": "26:00:00"
		              }
		              """;

		var customization = JsonSerializer.Deserialize<AppCustomization>(json);

		customization.Should().NotBeNull();
		customization.Title.Should().Be("MUDDIs Schicht Planner");
		customization.Subtitle.Should().Be("Auf eine HAMMER-mäßige Kieler Woche 2023!");
		customization.Contact.Should().Be("gastro@muddimarkt.org");
		customization.StartTimeSpan.TotalHours.Should().Be(8);
		customization.EndTimeSpan.TotalHours.Should().Be(26);
	}

	[Fact]
	public void ShouldDeserializeAppCustomization_WhenValid2()
	{
		string json = """
		              {
		                "Title": "MUDDIs Schicht Planner",
		                "Subtitle": "Auf eine HAMMER-mäßige Kieler Woche 2023!",
		                "Contact": "gastro@muddimarkt.org",
		                "StartTime": "08:00:00",
		                "EndTime": "1.02:00:00"
		              }
		              """;

		var customization = JsonSerializer.Deserialize<AppCustomization>(json);

		customization.Should().NotBeNull();
		customization.Title.Should().Be("MUDDIs Schicht Planner");
		customization.Subtitle.Should().Be("Auf eine HAMMER-mäßige Kieler Woche 2023!");
		customization.Contact.Should().Be("gastro@muddimarkt.org");
		customization.StartTimeSpan.TotalHours.Should().Be(8);
		customization.EndTimeSpan.TotalHours.Should().Be(26);
	}

	[Fact]
	public void ShouldDeserializeAppCustomization_WhenValid3()
	{
		string json = """
		              {
		                "Title": "MUDDIs Schicht Planner",
		                "Subtitle": "Auf eine HAMMER-mäßige Kieler Woche 2023!",
		                "Contact": "gastro@muddimarkt.org",
		                "StartTime": "08:00",
		                "EndTime": "26:00"
		              }
		              """;

		var customization = JsonSerializer.Deserialize<AppCustomization>(json);

		customization.Should().NotBeNull();
		customization.Title.Should().Be("MUDDIs Schicht Planner");
		customization.Subtitle.Should().Be("Auf eine HAMMER-mäßige Kieler Woche 2023!");
		customization.Contact.Should().Be("gastro@muddimarkt.org");
		customization.StartTimeSpan.TotalHours.Should().Be(8);
		customization.EndTimeSpan.TotalHours.Should().Be(26);
	}
}