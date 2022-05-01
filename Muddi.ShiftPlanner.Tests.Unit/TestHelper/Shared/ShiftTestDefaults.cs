using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Muddi.ShiftPlanner.Shared.Entities;
using Muddi.ShiftPlanner.Shared.Exceptions;
using Xunit;

// ReSharper disable once CheckNamespace
namespace Muddi.ShiftPlanner.Tests.Unit.Shared;

public static class ShiftTestDefaults
{
	internal static readonly ShiftRole DefaultRole = new(Guid.NewGuid(), "Default");
	internal  static readonly ShiftRole MuddiInChargeRole = new(Guid.NewGuid(), "Muddi In Charge");
	internal  static readonly ShiftRole TapRole = new(Guid.NewGuid(), "Zapfe");

	internal  static readonly WorkingUserBase UserOne = new TestUser("User One");
	internal  static readonly WorkingUserBase UserTwo = new TestUser("User Two");
	internal  static readonly WorkingUserBase UserThree = new TestUser("User Three");
	internal  static readonly WorkingUserBase UserFour = new TestUser("User Four");
	internal  static readonly WorkingUserBase UserFive = new TestUser("User Five");

	internal  static readonly Dictionary<ShiftRole, int> DefaultRolesDictionary = new()
	{
		[DefaultRole] = 2,
		[MuddiInChargeRole] = 1,
		[TapRole] = 1
	};

	

	
}