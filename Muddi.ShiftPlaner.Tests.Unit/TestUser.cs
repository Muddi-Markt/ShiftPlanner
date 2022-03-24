using System;
using Muddi.ShiftPlanner.Shared;
using Muddi.ShiftPlanner.Shared.Entities;

namespace Muddi.ShiftPlaner.Tests.Unit;

public class TestUser : WorkingUserBase
{
	public TestUser(string name) : base(Guid.NewGuid(), name)
	{
	}
}