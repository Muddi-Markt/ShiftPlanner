using System.Diagnostics.CodeAnalysis;
using FluentValidation.Results;
using Muddi.ShiftPlanner.Server.Api.Endpoints;
using Muddi.ShiftPlanner.Server.Api.Endpoints.Containers;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Server.Database.Entities;

namespace Muddi.ShiftPlanner.Server.Api.Services;

public static class ShiftService
{
	public static IEnumerable<GetShiftTypesResponse> GetAvailableShiftTypes(this ShiftContainer container, DateTime requestStartTime)
	{
		requestStartTime = requestStartTime.ToUniversalTime();
		var counter = container.Framework.ShiftTypeCounts.Select(sft => new ShiftTypeCountsHelper(sft)).ToList();
		foreach (var shift in container.Shifts.Where(s => s.Start == requestStartTime))
		{
			var q = counter.Single(c => c.Id == shift.Type.Id);
			q.Count--;
		}

		return counter
			.Where(c => c.Count > 0)
			.Select(c => new GetShiftTypesResponse { Id = c.Id, Name = c.Name });
	}

	private class ShiftTypeCountsHelper
	{
		public ShiftTypeCountsHelper(ShiftFrameworkTypeCount sft)
		{
			Id = sft.ShiftType.Id;
			Name = sft.ShiftType.Name;
			Count = sft.Count;
		}

		public string Name { get; }

		internal Guid Id { get; }
		internal int Count { get; set; }
	}

	public static ValidationFailure? PreAddShiftSanityCheck(this ShiftContainer container, CreateShiftRequest req)
	{
		if (container.Shifts.Any(s => s.EmployeeKeycloakId == req.EmployeeKeycloakId && s.Start == req.Start))
		{
			return new ValidationFailure("user", "User already has shift at given time");
		}

		var availableShiftTypes = container.GetAvailableShiftTypes(req.Start);
		if (availableShiftTypes.All(st => st.Id != req.ShiftTypeId))
		{
			return new ValidationFailure("ShiftType", "not available");
		}

		return null;
	}
}