using System.Runtime.Serialization;
using Muddi.ShiftPlanner.Shared.Entities;

namespace Muddi.ShiftPlanner.Shared.Exceptions;

public class MuddiException : Exception
{
	public MuddiException()
	{
	}

	protected MuddiException(SerializationInfo info, StreamingContext context) : base(info, context)
	{
	}

	public MuddiException(string? message) : base(message)
	{
	}

	public MuddiException(string? message, Exception? innerException) : base(message, innerException)
	{
	}
}


public class DateTimeNotUtcException : MuddiException
{
	public DateTimeNotUtcException(string caller = "Time") : base($"{caller} is not in UTC format")
	{
		
	}
}

public class TooManyWorkersException : MuddiException
{
	public TooManyWorkersException(Shift shift) : base($"Too many workers for role {shift.Role} applied")
	{
		
	}
}