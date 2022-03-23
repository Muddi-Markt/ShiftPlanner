using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Muddi.ShiftPlanner.Shared.Exceptions;

namespace Muddi.ShiftPlanner.Shared;

public static class DateTimeExtensions
{
	public static DateTime ThrowIfNotUtc(this DateTime dateTime, [CallerArgumentExpression("dateTime")] string name = "")
	{
		if (dateTime.Kind != DateTimeKind.Utc)
			throw new DateTimeNotUtcException(name);
		return dateTime;
	}
}