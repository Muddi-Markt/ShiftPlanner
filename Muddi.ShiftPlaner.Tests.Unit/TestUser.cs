using System;
using Muddi.ShiftPlanner.Shared;
using Muddi.ShiftPlanner.Shared.Entities;

namespace Muddi.ShiftPlaner.Tests.Unit;

public class TestUser : WorkingUserBase
{
	public TestUser(Guid keycloakId) : base(keycloakId)
	{
	}
}