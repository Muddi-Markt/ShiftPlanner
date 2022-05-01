using System;
using FluentAssertions;

// ReSharper disable EqualExpressionComparison

namespace Muddi.ShiftPlanner.Tests.Unit;

public static class EqualityChecker
{
	public static void Check<T>(T obj, T objNotEqual, T objNotSameReferenceButEqual) where T : class, IEquatable<T>
	{
		obj.Equals(obj).Should().BeTrue();
		obj.Equals((object?)null).Should().BeFalse();
		obj!.Equals((object?)obj).Should().BeTrue();
		obj.Equals((object?)objNotEqual).Should().BeFalse();
		obj.Equals(new { Test = "Obj" }).Should().BeFalse();
		obj.Equals(null).Should().BeFalse();
		(obj != objNotEqual).Should().BeTrue();
		(obj == obj).Should().BeTrue();
		obj!.Equals(objNotEqual).Should().BeFalse();
		obj.GetHashCode().Should().NotBe(objNotEqual.GetHashCode());
		// if (objNotSameReferenceButEqual is null)
		// return;
		obj.Equals((object?)objNotSameReferenceButEqual).Should().BeTrue();
		obj.Equals(objNotSameReferenceButEqual).Should().BeTrue();
	}
}