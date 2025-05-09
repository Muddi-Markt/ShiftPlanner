using System;
using System.IO;
using System.Text.Json;
using FluentAssertions;
using Muddi.ShiftPlanner.Shared;
using Muddi.ShiftPlanner.Shared.Exceptions;
using Muddi.ShiftPlanner.Shared.Json;
using Xunit;

namespace Muddi.ShiftPlanner.Tests.Unit;

public class ExtensionTests
{
	[Fact]
	public void ShouldSucceed_WhenTimeIsUtc()
	{
		var utcNow = DateTime.UtcNow;
		utcNow.ThrowIfNotUtc().Should().Be(utcNow);
	}

	[Fact]
	public void ShouldThrow_WhenTimeIsNotUtc()
	{
		var localNow = DateTime.Now.ToLocalTime();
		localNow.Invoking(l => l.ThrowIfNotUtc()).Should().Throw<DateTimeNotUtcException>();
	}
	[Fact]
	public void Read_ShouldDeserialize_WhenValidTimeFormat()
	{
		var converter = new TimeSpanHourFormatConverter();
		var json = JsonSerializer.Serialize("26:30:00");
		var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));
		reader.Read();

		var result = converter.Read(ref reader, typeof(TimeSpan), new JsonSerializerOptions());

		result.Should().Be(new TimeSpan(26, 30, 0));
	}
	
	[Fact]
	public void Read_ShouldDeserialize_WhenValidTimeFormatWithoutSeconds()
	{
		var converter = new TimeSpanHourFormatConverter();
		var json = JsonSerializer.Serialize("26:30");
		var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));
		reader.Read();

		var result = converter.Read(ref reader, typeof(TimeSpan), new JsonSerializerOptions());

		result.Should().Be(new TimeSpan(26, 30, 0));
	}


	[Fact]
	public void Write_ShouldSerialize_TimeSpan()
	{
		var converter = new TimeSpanHourFormatConverter();
		var value = new TimeSpan(12, 30, 0);
		using var stream = new MemoryStream();
		using var writer = new Utf8JsonWriter(stream);

		converter.Write(writer, value, new JsonSerializerOptions());
		writer.Flush();

		var json = System.Text.Encoding.UTF8.GetString(stream.ToArray());
		json.Should().Be("\"12:30:00\"");
	}

	[Fact]
	public void Read_ShouldThrow_WhenInvalidTimeFormat()
	{
		var converter = new TimeSpanHourFormatConverter();
		var json = JsonSerializer.Serialize("invalid");
		

		void LocalAction()
		{
			var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));
			reader.Read();
			converter.Read(ref reader, typeof(TimeSpan), new JsonSerializerOptions());
		}

		Action action = LocalAction;

		action.Should().Throw<JsonException>().WithMessage("Unable to parse TimeSpan: invalid");
	}
}