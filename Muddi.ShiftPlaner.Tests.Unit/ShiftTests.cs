using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Muddi.ShiftPlanner.Shared;
using Muddi.ShiftPlanner.Shared.Entities;
using Muddi.ShiftPlanner.Shared.Exceptions;
using Xunit;

namespace Muddi.ShiftPlaner.Tests.Unit;

public class ShiftTests
{
	private static readonly ShiftRole DefaultRole = new(Guid.NewGuid(), "Default");
	private static readonly ShiftRole MuddiInChargeRole = new(Guid.NewGuid(), "Muddi In Charge");
	private static readonly ShiftRole TapRole = new(Guid.NewGuid(), "Zapfer*In");

	private static readonly WorkingUserBase UserOne = new TestUser(Guid.NewGuid());
	private static readonly WorkingUserBase UserTwo = new TestUser(Guid.NewGuid());
	private static readonly WorkingUserBase UserThree = new TestUser(Guid.NewGuid());
	private static readonly WorkingUserBase UserFour = new TestUser(Guid.NewGuid());
	private static readonly WorkingUserBase UserFive = new TestUser(Guid.NewGuid());

	[Fact]
	public void ShiftLocation_FullTest_ShouldSucceed()
	{
		var secondContainerSecondShiftStart = new DateTime(2022, 03, 23, 3, 30, 0, DateTimeKind.Utc);
		var startTime = DateTimeOffset.Parse("22.03.2022 20:00:00", null, DateTimeStyles.AssumeUniversal).UtcDateTime;
		var shiftDuration = TimeSpan.FromMinutes(90);
		int shiftsThatDay = 4;
		int shiftsThatDay2 = 2;
		var roles = new Dictionary<ShiftRole, int>
		{
			[DefaultRole] = 2,
			[MuddiInChargeRole] = 1,
			[TapRole] = 1
		};
		var framework = new ShiftFramework(shiftDuration, roles);
		var shiftContainer1 = new ShiftContainer(framework, startTime, shiftsThatDay);
		var shiftContainer2 = new ShiftContainer(framework, startTime + shiftContainer1.TotalTime, shiftsThatDay2);
		var shiftLocation = new ShiftLocation("Bar 1", ShiftLocationTypes.Bar);

		shiftLocation.AddContainer(shiftContainer1);
		shiftLocation.AddContainer(shiftContainer2);
		shiftLocation.AddShift(new Shift(UserOne, startTime, DefaultRole));
		var shiftUser2 = new Shift(UserTwo, startTime, DefaultRole);
		shiftLocation.AddShift(shiftUser2);
		shiftLocation.RemoveShift(shiftUser2);
		shiftLocation.AddShift(shiftUser2);
		shiftLocation.AddShift(new Shift(UserThree, startTime, MuddiInChargeRole));
		shiftLocation.AddShift(new Shift(UserFour, startTime, TapRole));
		shiftLocation.AddShift(new Shift(UserFour, startTime.Add(shiftDuration), TapRole));
		shiftLocation.AddShift(new Shift(UserOne, startTime + shiftContainer1.TotalTime + shiftDuration, DefaultRole));
		var container2Shift2User2 = new Shift(UserTwo, startTime + shiftContainer1.TotalTime + shiftDuration, DefaultRole);
		shiftLocation.AddShift(container2Shift2User2);


		shiftLocation.Containers[0].Should().Be(shiftContainer1);
		shiftLocation.Containers[1].Should().Be(shiftContainer2);
		shiftContainer1.StartTime.Should().Be(startTime);
		shiftContainer2.StartTime.Should().Be(startTime + shiftContainer1.TotalTime);
		shiftContainer1.EndTime.Should().Be(startTime + shiftDuration * shiftsThatDay);
		shiftContainer2.EndTime.Should().Be(startTime + shiftContainer1.TotalTime + shiftDuration * shiftsThatDay2);
		shiftContainer1.TotalTime.Should().Be(shiftDuration * shiftsThatDay);
		shiftContainer2.TotalTime.Should().Be(shiftDuration * shiftsThatDay2);
		shiftContainer1.TotalShifts.Should().Be(shiftsThatDay);
		shiftContainer2.TotalShifts.Should().Be(shiftsThatDay2);
		var shifts = shiftContainer1.GetShiftsAtGivenTime(startTime);
		shifts.Should().HaveCount(4);
		shifts.Should().AllSatisfy(t => t.StartTime.Should().Be(startTime));
		var allShifts = shiftContainer1.GetAllShifts().ToArray();
		allShifts.Should().HaveCount(5);
		allShifts.Should().AllSatisfy(t => t.User.Should().BeOfType<TestUser>());
		shiftContainer2.GetAllShifts().Should().HaveCount(2);
		shiftContainer2.GetShiftsAtGivenTime(secondContainerSecondShiftStart).Should().HaveCount(2);
		Action[] act =
		{
			() => shiftLocation.AddShift(new Shift(UserFive, startTime, DefaultRole)),
			() => shiftLocation.AddShift(new Shift(UserFive, startTime, MuddiInChargeRole)),
			() => shiftLocation.AddShift(new Shift(UserFive, startTime, TapRole)),
			() => shiftLocation.AddShift(new Shift(UserFive, secondContainerSecondShiftStart, DefaultRole))
		};
		act.Should().AllSatisfy(a => a.Should().Throw<TooManyWorkersException>());
		shiftLocation.RemoveShift(container2Shift2User2);
		Action act2 =
			() => shiftLocation.AddShift(new Shift(UserOne, secondContainerSecondShiftStart, DefaultRole));
		act2.Should().Throw<AmbiguousMatchException>();
	}
}