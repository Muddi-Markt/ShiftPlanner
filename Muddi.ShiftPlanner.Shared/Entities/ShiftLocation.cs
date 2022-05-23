using System.Reflection;
using Muddi.ShiftPlanner.Shared.Api;
using Muddi.ShiftPlanner.Shared.Exceptions;

namespace Muddi.ShiftPlanner.Shared.Entities;

public class ShiftLocation
{
	public static string LocationsPath = "/locations/{0}";
	public Guid Id { get; }
	public string Name { get; }
	public GetLocationTypesResponse Type { get; }
	public string Path { get; }
	public string? Icon { get; }
	public IReadOnlyList<ShiftContainer> Containers => _containers;

	private readonly List<ShiftContainer> _containers;

	public ShiftLocation(Guid id, string name, GetLocationTypesResponse type, IEnumerable<ShiftContainer> shiftContainers)
	{
		Name = name;
		Type = type;
		Id = id;
		Path = string.Format(LocationsPath, Id);
		_containers = new(shiftContainers);
	}

	public void AddContainer(ShiftContainer container)
	{
		if (_containers.FirstOrDefault(t =>
			    (t.StartTime >= container.StartTime && t.StartTime < container.EndTime)
			    || (t.EndTime > container.StartTime && t.EndTime <= container.EndTime)) is { } overlapContainer)
			throw new ContainerTimeOverlapsException(container, overlapContainer);
		_containers.Add(container);
	}

	public IEnumerable<Shift> GetAllShifts()
	{
		return Containers.SelectMany(t => t.GetAllShifts());
	}

	public void RemoveShift(Shift shift)
	{
		if (GetShiftContainer(shift) is not { } container)
			throw new StartTimeNotInContainerException(shift.StartTime);
		container.RemoveShift(shift);
	}

	public ShiftContainer? GetShiftContainerByTime(DateTime startTime) =>
		_containers.SingleOrDefault(t => t.IsTimeWithinContainer(startTime))
		?? null;

	public ShiftContainer GetContainerFromShift(Shift shift)
		=> _containers.FirstOrDefault(q => q.GetAllShifts().Any(t => t == shift))
		   ?? throw new ShiftNotInContainerException(shift);

	private ShiftContainer? GetShiftContainer(Shift shift) => GetShiftContainerByTime(shift.StartTime);

	public void UpdateShift(Shift shift, ShiftType type, EmployeeBase user)
	{
		throw new NotImplementedException();
		// new Shift()
		// GetShiftContainer(shift).UpdateShift(shift,)
	}
}