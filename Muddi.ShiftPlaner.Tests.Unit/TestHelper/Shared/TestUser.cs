using System;
using Muddi.ShiftPlanner.Shared;
using Muddi.ShiftPlanner.Shared.Entities;

// ReSharper disable once CheckNamespace
namespace Muddi.ShiftPlaner.Tests.Unit.Shared;

public class TestUser : WorkingUserBase, IEquatable<TestUser>
{
	public TestUser(string name) : base(Guid.NewGuid(), name)
	{
	}
	public TestUser(string name,Guid guid) : base(guid, name)
	{
	}

	public bool Equals(TestUser? other) => Equals((WorkingUserBase?)other);
}