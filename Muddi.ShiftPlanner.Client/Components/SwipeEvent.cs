namespace Muddi.ShiftPlanner.Client.Components;

public enum SwipeEvent
{
	Up,
	Down,
	Left,
	Right
}

[Flags]
public enum SwipeOrientation
{
	None,
	Horizontal,
	Vertical
}