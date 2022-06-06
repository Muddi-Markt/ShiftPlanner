using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using FluentValidation.Results;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Api.Endpoints;
using Muddi.ShiftPlanner.Server.Api.Endpoints.Containers;
using Muddi.ShiftPlanner.Server.Api.Exceptions;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Server.Database.Entities;
using Muddi.ShiftPlanner.Shared.Contracts.v1;

namespace Muddi.ShiftPlanner.Server.Api.Services;

public static class ShiftService
{
	public static IEnumerable<DateTime> GetStartTimes(this ShiftContainerEntity container)
	{
		var current = container.Start;
		for (var i = 0; i < container.TotalShifts; i++)
		{
			yield return current;
			current = current.Add(container.Framework.TimePerShift);
		}
	}

	public static GetShiftTypesCountResponse ToResponse(this ShiftFrameworkTypeCountEntity sft, DateTime startTime, DateTime endTime,
		Guid locationId)
	{
		return new GetShiftTypesCountResponse
		{
			Type = sft.ShiftType.Adapt<GetShiftTypesResponse>(),
			AvailableCount = sft.Count,
			TotalCount = sft.Count,
			Start = startTime,
			End = endTime,
			LocationId = locationId
		};
	}

	public static IEnumerable<GetShiftTypesCountResponse> GetAvailableShiftTypes(this ShiftContainerEntity container, DateTime startTime)
	{
		startTime = startTime.ToUniversalTime();
		var endTime = startTime + container.Framework.TimePerShift;
		var counter = container.Framework.ShiftTypeCounts
			.Select(sft => sft.ToResponse(startTime, endTime, container.Location.Id))
			.ToList();
		foreach (var shift in container.Shifts.Where(s => s.Start == startTime))
		{
			var q = counter.Single(c => c.Type.Id == shift.Type.Id);
			q.AvailableCount--;
		}

		return counter;
	}

	public static async Task<ValidationFailure?> PreAddShiftSanityCheck(this ShiftPlannerContext db,
		ShiftContainerEntity container,
		CreateShiftRequest req,
		ClaimsPrincipal user,
		Guid ignoreUserHasShiftAtGivenTimeWhenShiftType = default)
	{
		var shiftAtGivenTime = await db.Shifts
			.Include(s => s.ShiftContainer)
			.ThenInclude(c => c.Location)
			.AsNoTracking()
			.FirstOrDefaultAsync(s => s.EmployeeKeycloakId == req.EmployeeKeycloakId && s.Start == req.Start
			                                                                         && s.Type.Id !=
			                                                                         ignoreUserHasShiftAtGivenTimeWhenShiftType);

		if (shiftAtGivenTime is not null)
		{
			return new AlreadyShiftAtGivenTimeFailure(shiftAtGivenTime);
		}

		var availableShiftTypes = container.GetAvailableShiftTypes(req.Start);
		var shiftType = availableShiftTypes.FirstOrDefault(st => st.Type.Id == req.ShiftTypeId);
		if (shiftType is null)
			return new ValidationFailure("ShiftType", "not available");
		if (shiftType.Type.OnlyAssignableByAdmin && !user.IsInRole(ApiRoles.Admin))
			return new ValidationFailure("AdminRole", "Only available for admins");

		return null;
	}
	
}