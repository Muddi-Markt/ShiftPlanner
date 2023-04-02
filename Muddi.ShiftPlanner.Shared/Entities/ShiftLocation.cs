using System.Reflection;
using Muddi.ShiftPlanner.Shared.Api;
using Muddi.ShiftPlanner.Shared.Exceptions;

namespace Muddi.ShiftPlanner.Shared.Entities;

public class ShiftLocation
{
	private const string LocationsPath = "/locations/{0}";
	public Guid Id { get; }
	public string Name { get; }
	public GetLocationTypesResponse Type { get; }
	public string Path { get; }
	public IReadOnlyList<ShiftContainer> Containers => _containers;

	private readonly List<ShiftContainer> _containers;
	public int TotalShifts { get; }
	public int AssignedShifts { get; }
	public int PercentageAlreadyAssignedShifts =>
		TotalShifts == 0
			? -1
			: Convert.ToInt32(Math.Min(Math.Round((double)AssignedShifts / TotalShifts * 100.0), 100.0));

	public ShiftLocation(Guid id, string name, GetLocationTypesResponse type, IEnumerable<ShiftContainer> shiftContainers, int totalShifts,
		int assignedShifts)
	{
		Name = name;
		Type = type;
		TotalShifts = totalShifts;
		AssignedShifts = Math.Min(totalShifts, assignedShifts);
		Id = id;
		Path = string.Format(LocationsPath, Id);
		_containers = new(shiftContainers);
	}

	public ShiftContainer? GetShiftContainerByTime(DateTime startTime) =>
		_containers.SingleOrDefault(t => t.IsTimeWithinContainer(startTime))
		?? null;
}