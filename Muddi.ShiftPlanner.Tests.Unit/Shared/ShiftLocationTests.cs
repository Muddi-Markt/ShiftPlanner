using System;
using FluentAssertions;
using Muddi.ShiftPlanner.Shared.Entities;
using Xunit;

namespace Muddi.ShiftPlanner.Tests.Unit.Shared;

public class ShiftLocationTests
{
	[Fact]
	public void ShouldSucceed_WhenFilledWithInfos()
	{
		var shiftLocation = new ShiftLocation("TestName", ShiftLocationTypes.InfoStand);
		shiftLocation.Id.Should().NotBe(Guid.Empty);
		shiftLocation.Name.Should().Be("TestName");
		shiftLocation.Type.Should().Be(ShiftLocationTypes.InfoStand);
		shiftLocation.Path.Should().Be(string.Format(ShiftLocation.LocationsPath, shiftLocation.Id));
		shiftLocation.Icon.Should().BeNull();
		shiftLocation.Containers.Should().BeEmpty();
	}
}