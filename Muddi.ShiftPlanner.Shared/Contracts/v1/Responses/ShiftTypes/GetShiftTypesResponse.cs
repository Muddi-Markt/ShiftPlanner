﻿namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Responses;

public class GetShiftTypesResponse : IMuddiResponse
{
	public Guid Id { get; set; }
	public string Name { get; set; }
}