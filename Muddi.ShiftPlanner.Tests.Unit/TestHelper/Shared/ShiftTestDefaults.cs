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
	internal static readonly ShiftType DefaultType = new(Guid.NewGuid(), "Default");
	internal  static readonly ShiftType MuddiInChargeType = new(Guid.NewGuid(), "Muddi In Charge");
	internal  static readonly ShiftType TapType = new(Guid.NewGuid(), "Zapfe");

	internal  static readonly EmployeeBase UserOne = new TestUser("User One");
	internal  static readonly EmployeeBase UserTwo = new TestUser("User Two");
	internal  static readonly EmployeeBase UserThree = new TestUser("User Three");
	internal  static readonly EmployeeBase UserFour = new TestUser("User Four");
	internal  static readonly EmployeeBase UserFive = new TestUser("User Five");

	internal  static readonly Dictionary<ShiftType, int> DefaultRolesDictionary = new()
	{
		[DefaultType] = 2,
		[MuddiInChargeType] = 1,
		[TapType] = 1
	};

	

	
}