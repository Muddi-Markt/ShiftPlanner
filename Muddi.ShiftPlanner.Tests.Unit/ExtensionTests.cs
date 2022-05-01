using System;
using FluentAssertions;
using Muddi.ShiftPlanner.Shared;
using Muddi.ShiftPlanner.Shared.Exceptions;
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

}