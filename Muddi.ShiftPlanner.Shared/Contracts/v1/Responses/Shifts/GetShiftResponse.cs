﻿namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Responses;

public class GetShiftResponse : IMuddiResponse
{
	public Guid Id { get; set; }
	public Guid ContainerId { get; set; }
	public GetEmployeeResponse Employee { get; set; }
	public DateTime Start { get; set; }
	public DateTime End { get; set; }
	public GetShiftTypesResponse? Type { get; set; }
}