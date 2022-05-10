using Muddi.ShiftPlanner.Shared.Entities;

namespace Muddi.ShiftPlanner.Shared;

public static class MapperExtensions
{
	public static ShiftLocation MapToShiftLocation(this GetLocationResponse dto)
	{
		var containers = dto.Containers.Select(t =>
		{
			var shiftTypeCount =
				t.Framework.ShiftTypeCounts.ToDictionary(k => k.ShiftType.MapToShiftType(), v => v.Count);
			var framework = new ShiftFramework(t.Framework.Id, TimeSpan.FromSeconds(t.Framework.SecondsPerShift), shiftTypeCount);
			return new ShiftContainer(t.Id, framework, t.Start, t.TotalShifts);
		});
		return new ShiftLocation(dto.Id, dto.Name, dto.Type, containers);
	}

	public static Shift MapToShift(this GetShiftResponse dto)
	{
		return new Shift(new Employee(dto.Employee.Id, dto.Employee.UserName), dto.Start, dto.End, dto.Type.MapToShiftType());
	}

	public static ShiftType MapToShiftType(this GetShiftTypesResponse dto)
		=> new ShiftType(dto.Id, dto.Name);
}