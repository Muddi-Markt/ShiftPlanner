﻿namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Responses;

public class GetContainerResponse : IMuddiResponse
{
	public Guid Id { get; set; }
	public DateTime	Start { get; set; }
	public int TotalShifts { get; set; }
	public string Color { get; set; }
	public GetFrameworkResponse Framework { get; set; }
}