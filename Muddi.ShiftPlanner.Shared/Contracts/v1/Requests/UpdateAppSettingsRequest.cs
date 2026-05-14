namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Requests;

public class UpdateAppSettingsRequest
{
	/// <summary>
	/// Daily start time for shift containers (e.g. "08:00:00")
	/// </summary>
	public TimeSpan StartTime { get; init; } = new(8, 0, 0);

	/// <summary>
	/// Daily end time for shift containers (e.g. "26:00:00" for 2:00 AM next day)
	/// </summary>
	public TimeSpan EndTime { get; init; } = new(26, 0, 0);
}