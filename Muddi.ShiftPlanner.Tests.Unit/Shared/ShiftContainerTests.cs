using System;
using FluentAssertions;
using Muddi.ShiftPlanner.Shared.Entities;
using Muddi.ShiftPlanner.Shared.Exceptions;
using Xunit;

namespace Muddi.ShiftPlanner.Tests.Unit.Shared;

using static ShiftTestDefaults;

public class ShiftContainerTests
{
	[Fact]
	public void ShouldSucceed_WhenFilledWithInfos()
	{
	}


	[Fact]
	public void ShouldThrow_WhenContainerOverlaps()
	{
		var shiftStart = new DateTime(2022, 03, 22, 20, 00, 0, DateTimeKind.Utc);
		var shiftDuration = TimeSpan.FromMinutes(90);
		int shiftsThatDay = 4;
		var framework = new ShiftFramework(shiftDuration, DefaultRolesDictionary);
		var shiftMainContainer = new ShiftContainer(framework, shiftStart, shiftsThatDay);
		var shiftShouldThrowContainers = new ShiftContainer[]
		{
			new(framework, shiftStart, shiftsThatDay),
			new(framework, shiftStart + shiftDuration, shiftsThatDay),
			new(framework, shiftMainContainer.EndTime - shiftDuration, shiftsThatDay),
			new(framework, shiftStart - shiftDuration, shiftsThatDay),
			new(framework, shiftStart - shiftDuration, shiftsThatDay * 2)
		};
		var shiftLocation = new ShiftLocation("Bar 1", ShiftLocationTypes.Bar);

		shiftLocation.AddContainer(shiftMainContainer);
		shiftShouldThrowContainers.Should().AllSatisfy(
			container => shiftLocation.Invoking(sl => sl.AddContainer(container))
				.Should().Throw<ContainerTimeOverlapsException>());
	}
}