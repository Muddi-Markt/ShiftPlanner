﻿namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Requests;

public class CreateLocationTypeRequest
{
	public string Name { get; set; }
	public bool AdminOnly { get; set; } = false;
}