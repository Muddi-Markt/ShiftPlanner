using System;
using System.Reflection.Emit;
using FluentAssertions;
using Muddi.ShiftPlanner.Shared.Entities;
using Xunit;

namespace Muddi.ShiftPlaner.Tests.Unit.Shared;

public class WorkingUserTests
{
	[Fact]
	public void ShouldSucceed_WhenFilledWithInfos()
	{
		var user1 = new TestUser("TestUser");

		var user2 = new TestUser("TestUser");
		var user3 = new TestUser("TestUser", user1.KeycloakId);
		user1.Name.Should().Be("TestUser");
		user1.KeycloakId.Should().NotBe(Guid.Empty);
		user1.UserRole.Should().Be(UserRoles.Worker);
		user1.ToString().Should().Be($"{user1.Name} ({user1.KeycloakId})");
		EqualityChecker.Check(user1, user2, user3);
		(user1 == user3).Should().BeTrue();
		(user1 != user1).Should().BeFalse();
		(user1 == user2).Should().BeFalse();
	}
}