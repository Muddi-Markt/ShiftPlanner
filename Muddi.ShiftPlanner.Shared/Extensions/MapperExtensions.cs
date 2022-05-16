using Muddi.ShiftPlanner.Shared.Entities;

namespace Muddi.ShiftPlanner.Shared;

public static class MapperExtensions
{
	public static ShiftLocation MapToShiftLocation(this GetLocationResponse dto)
	{
		var containers = dto.Containers?.Select(t =>
		{
			var shiftTypeCount =
				t.Framework.ShiftTypeCounts.ToDictionary(k => k.ShiftType.MapToShiftType(), v => v.Count);
			var framework = new ShiftFramework(t.Framework.Id, TimeSpan.FromSeconds(t.Framework.SecondsPerShift), shiftTypeCount);
			return new ShiftContainer(t.Id, framework, t.Start, t.TotalShifts);
		});
		return new ShiftLocation(dto.Id, dto.Name, dto.Type, containers ?? Enumerable.Empty<ShiftContainer>());
	}

	public static Shift MapToShift(this GetShiftResponse dto)
	{
		return new Shift(dto.Id, new Employee(dto.Employee.Id, dto.Employee.UserName),
			dto.Start.ToUniversalTime().ToLocalTime(),
			dto.End.ToUniversalTime().ToLocalTime(),
			dto.Type.MapToShiftType(),
			dto.ContainerId
		);
	}
	public static GetShiftResponse MapToShiftResponse(this Shift dto)
	{
		return new GetShiftResponse
		{
			Id = dto.Id,
			ContainerId = dto.ContainerId,
			Employee = new(){UserName = dto.User.Name, Id = dto.User.KeycloakId},
			Start = dto.StartTime,
			End = dto.EndTime,
			Type = new(){Id = dto.Type.Id, Name = dto.Type.Name}
		};
	}

	public static ShiftType MapToShiftType(this GetShiftTypesResponse dto)
		=> new ShiftType(dto.Id, dto.Name);

	public static Shift ToLocalTime(this Shift shift)
		=> new(shift.Id,
			shift.User, 
			shift.StartTime.ToUniversalTime().ToLocalTime(), 
			shift.EndTime.ToUniversalTime().ToLocalTime(), 
			shift.Type,
			shift.ContainerId);
}