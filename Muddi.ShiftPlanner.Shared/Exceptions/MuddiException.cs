using System.Runtime.Serialization;
using Muddi.ShiftPlanner.Shared.Entities;

namespace Muddi.ShiftPlanner.Shared.Exceptions;

public class DateTimeNotUtcException : MuddiException
{
	public DateTimeNotUtcException(string caller = "Time")
		: base($"{caller} is not in UTC format")
	{
	}
}

public class TooManyWorkersException : MuddiException
{
	public TooManyWorkersException(Shift shift)
		: base($"Too many workers for role {shift.Type} applied")
	{
	}
}

public class UserAlreadyAssignedException : MuddiException
{
	public UserAlreadyAssignedException(Shift shift)
		: base($"Shift with start time {shift.StartTime} and user {shift.User} already assigned")
	{
	}
}

public class StartTimeNotInContainerException : MuddiException
{
	public StartTimeNotInContainerException(DateTime shift)
		: base($"Start time {shift} is not valid for this shift container")
	{
	}
}

public class ContainerTimeOverlapsException : MuddiException
{
	public ContainerTimeOverlapsException(ShiftContainer container, ShiftContainer overlapContainer)
		: base($"Container starting at {container.StartTime} and ends at {container.EndTime} " +
		       "overlaps with container " +
		       $"starting at {overlapContainer.StartTime} and ends at {overlapContainer.EndTime}")
	{
	}
}

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