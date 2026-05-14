namespace Muddi.ShiftPlanner.Shared.Entities;

/// <summary>
/// App-wide settings stored as JSONB in the database.
/// Used for settings that should be editable via the UI (StartTime, EndTime, etc.).
/// </summary>
public record ApplicationSettings
{
	/// <summary>
	/// Daily start time for shift containers. Defaults to 08:00.
	/// </summary>
	public TimeSpan StartTime { get; init; } = new(8, 0, 0);

	/// <summary>
	/// Daily end time for shift containers. Defaults to 26:00 (02:00 the next day).
	/// </summary>
	public TimeSpan EndTime { get; init; } = new(26, 0, 0);
}
