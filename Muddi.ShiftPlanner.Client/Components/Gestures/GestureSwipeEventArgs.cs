using Microsoft.AspNetCore.Components.Web;

namespace Muddi.ShiftPlanner.Client.Components.Gestures;

public record GestureSwipeEventArgs : GestureEventArgs
{
	public double? Angle { get; init; }
	public GestureDirection Direction { get; init; }
	public double DistanceX { get; init; }
	public double DistanceY { get; init; }
	public double Factor { get; init; }
}

public abstract record GestureEventArgs
{
	public string? Type { get; init; }
	public TouchPoint[]? StartPoints { get; init; }
	public TouchPoint[]? CurrentPoints { get; init; }
	public int GestureCount { get; init; }
	public int GestureDuration { get; init; }
}