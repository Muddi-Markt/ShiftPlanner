﻿using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses;

namespace Muddi.ShiftPlanner.Client;

public static class GetShiftResponseExtensions
{
	public static string StartToDisplayString(this GetShiftResponse response)
	{
		return (response.Start + (response.Type?.StartingTimeShift ?? TimeSpan.Zero)).ToLocalTime().ToString("HH:mm");
	}

	public static string EndToDisplayString(this GetShiftResponse response)
	{
		return (response.End + (response.Type?.StartingTimeShift ?? TimeSpan.Zero)).ToLocalTime().ToString("HH:mm");
	}
}