namespace Muddi.ShiftPlanner.Shared.Entities;

public class ShiftLocation
{
	public Guid Id { get; }
	public string Name { get; }
	public ShiftLocationTypes Type { get; }
	public string Path { get; }
	public bool Expanded { get; set; }
	public string Icon { get; }
	public IReadOnlyList<ShiftContainer> Containers => _containers;
	private readonly List<ShiftContainer> _containers = new();

	public ShiftLocation(string name, ShiftLocationTypes type)
	{
		Name = name;
		Type = type;
		Id = Guid.NewGuid(); //TODO get from database
		Path = "/locations/" + Id;
	}

	public void AddContainer(ShiftContainer container)
	{
		_containers.Add(container);
	}

	public void AddShift(Shift shift)
	{
		GetShiftContainer(shift).AddShift(shift);
	}

	public void RemoveShift(Shift shift)
	{
		GetShiftContainer(shift).RemoveShift(shift);
	}

	private ShiftContainer GetShiftContainer(Shift shift) => _containers.Single(t => t.IsShiftInContainer(shift));
}

public enum ShiftLocationTypes
{
	Unknown,
	Bar,
	InfoStand
}