using System;
using System.Linq;
using FluentAssertions;
using Muddi.ShiftPlanner.Shared.Entities;
using Muddi.ShiftPlanner.Shared.Exceptions;
using Xunit;

namespace Muddi.ShiftPlanner.Tests.Unit.Shared;

using static ShiftTestDefaults;

public class GlobalShiftTests
{
	[Fact]
	public void ShouldSucceed_WithFullTest()
	{
		var secondContainerSecondShiftStart = new DateTime(2022, 03, 23, 3, 30, 0, DateTimeKind.Utc);
		var firstContainerShiftStart = new DateTime(2022, 03, 22, 20, 00, 0, DateTimeKind.Utc);
		var shiftDuration = TimeSpan.FromMinutes(90);
		int shiftsThatDay = 4;
		int shiftsThatDay2 = 2;
		var framework = new ShiftFramework(shiftDuration, DefaultRolesDictionary);
		var shiftContainer1 = new ShiftContainer(framework, firstContainerShiftStart, shiftsThatDay);
		var shiftContainer2 = new ShiftContainer(framework, firstContainerShiftStart + shiftContainer1.TotalTime, shiftsThatDay2);
		var shiftLocation = new ShiftLocation("Bar 1", ShiftLocationTypes.Bar);

		Action[] actionsShouldThrowTooManyWorkers =
		{
			() => shiftLocation.AddShift(UserFive, firstContainerShiftStart, DefaultType),
			() => shiftLocation.AddShift(UserFive, firstContainerShiftStart, MuddiInChargeType),
			() => shiftLocation.AddShift(UserFive, firstContainerShiftStart, TapType),
			() => shiftLocation.AddShift(UserFive, secondContainerSecondShiftStart, DefaultType)
		};
		Action[] actionShouldThrowAmbiguousMath =
		{
			() => shiftLocation.AddShift(UserOne, secondContainerSecondShiftStart, DefaultType),
			() => shiftLocation.AddShift(UserOne, firstContainerShiftStart, DefaultType),
		};
		Action[] actionsShouldThrowStartTimeNotInContainer =
		{
			() => shiftLocation.AddShift(UserFive, secondContainerSecondShiftStart.AddMinutes(1), TapType),
			() => shiftLocation.AddShift(UserFive, firstContainerShiftStart.Add(shiftDuration * -1), TapType),
		};

		shiftLocation.AddContainer(shiftContainer1);
		shiftLocation.AddContainer(shiftContainer2);
		shiftLocation.AddShift(UserOne, firstContainerShiftStart, DefaultType);
		var shiftUser2 = shiftLocation.AddShift(UserTwo, firstContainerShiftStart, DefaultType);
		shiftLocation.RemoveShift(shiftUser2);
		shiftLocation.AddShift(UserTwo, firstContainerShiftStart, DefaultType);
		shiftLocation.AddShift(UserThree, firstContainerShiftStart, MuddiInChargeType);
		shiftLocation.AddShift(UserFour, firstContainerShiftStart, TapType);
		shiftLocation.AddShift(UserFour, firstContainerShiftStart.Add(shiftDuration), TapType);
		shiftLocation.AddShift(UserOne, firstContainerShiftStart + shiftContainer1.TotalTime + shiftDuration, DefaultType);
		var container2Shift2User2 =
			shiftLocation.AddShift(UserTwo, firstContainerShiftStart + shiftContainer1.TotalTime + shiftDuration, DefaultType);


		shiftContainer1.ShiftStartTimes.Should().HaveCount(4);
		shiftContainer2.ShiftStartTimes.Should().HaveCount(2);
		var allLocationShifts = shiftLocation.GetAllShifts();
		allLocationShifts.Should().HaveCount(7);
		shiftLocation.Containers[0].Should().Be(shiftContainer1);
		shiftLocation.Containers[1].Should().Be(shiftContainer2);
		shiftContainer1.StartTime.Should().Be(firstContainerShiftStart);
		shiftContainer2.StartTime.Should().Be(firstContainerShiftStart + shiftContainer1.TotalTime);
		shiftContainer1.EndTime.Should().Be(firstContainerShiftStart + shiftDuration * shiftsThatDay);
		shiftContainer2.EndTime.Should().Be(firstContainerShiftStart + shiftContainer1.TotalTime + shiftDuration * shiftsThatDay2);
		shiftContainer1.TotalTime.Should().Be(shiftDuration * shiftsThatDay);
		shiftContainer2.TotalTime.Should().Be(shiftDuration * shiftsThatDay2);
		shiftContainer1.TotalShifts.Should().Be(shiftsThatDay);
		shiftContainer2.TotalShifts.Should().Be(shiftsThatDay2);
		var shifts = shiftContainer1.GetShiftsAtGivenTime(firstContainerShiftStart);
		shifts.Should().HaveCount(4);
		shifts.Should().AllSatisfy(t => t.StartTime.Should().Be(firstContainerShiftStart));
		var allShifts = shiftContainer1.GetAllShifts().ToArray();
		allShifts.Should().HaveCount(5);
		allShifts.Should().AllSatisfy(t => t.User.Should().BeOfType<TestUser>());
		shiftContainer2.GetAllShifts().Should().HaveCount(2);
		shiftContainer2.GetShiftsAtGivenTime(secondContainerSecondShiftStart).Should().HaveCount(2);

		shiftContainer1.GetAvailableRolesAtGivenTime(firstContainerShiftStart).Should().HaveCount(0);
		shiftContainer1.GetAvailableRolesAtGivenTime(firstContainerShiftStart + shiftDuration).Should().HaveCount(2);

		shiftContainer1.IsTimeWithinContainer(firstContainerShiftStart + shiftDuration).Should().BeTrue();
		shiftContainer1.GetBestShiftStartTimeForTime(firstContainerShiftStart.AddMinutes(2)).Should().Be(firstContainerShiftStart);
		shiftContainer1.GetBestShiftStartTimeForTime(firstContainerShiftStart + shiftDuration.Subtract(TimeSpan.FromMinutes(1))).Should()
			.Be(firstContainerShiftStart);
		shiftContainer1.GetBestShiftStartTimeForTime(firstContainerShiftStart + shiftDuration.Add(TimeSpan.FromMinutes(1))).Should()
			.Be(firstContainerShiftStart + shiftDuration);
		shiftContainer1.GetBestShiftStartTimeForTime(secondContainerSecondShiftStart + shiftDuration).Should()
			.Be(shiftContainer1.EndTime - shiftDuration);

		actionsShouldThrowTooManyWorkers.Should()
			.AllSatisfy(a => a.Should().Throw<TooManyWorkersException>());
		actionShouldThrowAmbiguousMath.Should()
			.AllSatisfy(a => a.Should().Throw<UserAlreadyAssignedException>());
		actionsShouldThrowStartTimeNotInContainer.Should()
			.AllSatisfy(a => a.Should().Throw<StartTimeNotInContainerException>());
	}
}