using System.Reflection;
using Muddi.ShiftPlanner.Shared.Exceptions;

namespace Muddi.ShiftPlanner.Shared.Entities;

public class ShiftLocation
{
	public static string LocationsPath = "/locations/{0}";
	public Guid Id { get; }
	public string Name { get; }
	public ShiftLocationTypes Type { get; }
	public string Path { get; }
	public string? Icon { get; }
	public IReadOnlyList<ShiftContainer> Containers => _containers;
	
	private readonly List<ShiftContainer> _containers = new();

	public ShiftLocation(string name, ShiftLocationTypes type)
	{
		Name = name;
		Type = type;
		Id = Guid.NewGuid(); //TODO get from database
		Path = string.Format(LocationsPath, Id);
	}

	public void AddContainer(ShiftContainer container)
	{
		if (_containers.FirstOrDefault(t =>
			    (t.StartTime >= container.StartTime && t.StartTime < container.EndTime)
			    || (t.EndTime > container.StartTime && t.EndTime <= container.EndTime)) is { } overlapContainer)
			throw new ContainerTimeOverlapsException(container, overlapContainer);
		_containers.Add(container);
	}

	public Shift AddShift(WorkingUserBase user, DateTime start, ShiftRole role)
	{
		var container = GetShiftContainerByTime(start);
		var shift = new Shift(user, start, start + container.Framework.TimePerShift, role);
		container.AddShift(shift);
		return shift;
	}

	public IEnumerable<Shift> GetAllShifts()
	{
		return Containers.SelectMany(t => t.GetAllShifts());
	}

	public void RemoveShift(Shift shift) => GetShiftContainer(shift).RemoveShift(shift);

	public ShiftContainer GetShiftContainerByTime(DateTime startTime) =>
		_containers.SingleOrDefault(t => t.IsTimeWithinContainer(startTime))
		?? throw new StartTimeNotInContainerException(startTime);

	private ShiftContainer GetShiftContainer(Shift shift) => GetShiftContainerByTime(shift.StartTime);

	public void UpdateShift(Shift shift, ShiftRole role, WorkingUserBase user)
	{
		throw new NotImplementedException();
		// new Shift()
		// GetShiftContainer(shift).UpdateShift(shift,)
	}
}

public enum ShiftLocationTypes
{
	Unknown,
	Bar,
	InfoStand
}