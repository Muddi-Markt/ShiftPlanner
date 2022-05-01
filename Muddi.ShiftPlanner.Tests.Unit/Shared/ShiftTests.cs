using System;
using FluentAssertions;
using Muddi.ShiftPlanner.Shared.Entities;
using Xunit;

namespace Muddi.ShiftPlanner.Tests.Unit.Shared;

using static ShiftTestDefaults;

public class ShiftTests
{
	[Fact]
	public void ShouldSucceed_WhenFilledWithInfos()
	{
		var startTime = new DateTime(2022, 07, 23, 12, 00, 00);
		var timeSpan = TimeSpan.FromMinutes(90);
		var shift = new Shift(UserOne, startTime, startTime + timeSpan, DefaultRole);
		var shift2 = new Shift(UserTwo, startTime, startTime + timeSpan, DefaultRole);
		var shift3 = new Shift(UserOne, startTime, startTime + timeSpan, DefaultRole);
		shift.Title.Should().StartWith(UserOne.Name.Split(' ')[0]);
		shift.EndTime.Should().Be(startTime + timeSpan);
		EqualityChecker.Check(shift, shift2, shift3);
		(shift == shift3).Should().BeTrue();
		// ReSharper disable once EqualExpressionComparison
		(shift != shift).Should().BeFalse();
		(shift == shift2).Should().BeFalse();
	}
}