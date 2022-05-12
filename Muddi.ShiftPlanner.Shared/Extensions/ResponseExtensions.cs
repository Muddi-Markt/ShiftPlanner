namespace Muddi.ShiftPlanner.Shared;

public static class ResponseExtensions
{
	public static DateTime GetEndTime(this GetContainerResponse c)
	{
		if (c.Framework is null)
			return default;
		return c.Start + TimeSpan.FromSeconds(c.Framework.SecondsPerShift * c.TotalShifts);
	}

	public static string GetRelativeUri(this GetLocationResponse l)
		=> $"/locations/{l.Id}";
}