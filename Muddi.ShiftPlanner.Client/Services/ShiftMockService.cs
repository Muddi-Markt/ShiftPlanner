using Muddi.ShiftPlanner.Shared;
using Muddi.ShiftPlanner.Shared.Entities;

namespace Muddi.ShiftPlanner.Client.Services;

public class ShiftMockService
{
	private static readonly ShiftLocation[] ShiftPlaces;

	private static ShiftRole RoleBeverages = new ShiftRole(Guid.NewGuid(), "Getränke");
	private static ShiftRole RoleCash = new ShiftRole(Guid.NewGuid(), "Kasse");
	private static ShiftRole RoleTap = new ShiftRole(Guid.NewGuid(), "Zapfen");
	private static ShiftRole RoleMuddiInCharge = new ShiftRole(Guid.NewGuid(), "Muddi in Charge");

	static ShiftMockService()
	{
		ShiftPlaces = new ShiftLocation[]
		{
			new("Bar 1", ShiftLocationTypes.Bar),
			new("Bar 2", ShiftLocationTypes.Bar),
			new("Infostand", ShiftLocationTypes.InfoStand)
		};
		var defaultRoles = new Dictionary<ShiftRole, int>
		{
			[RoleBeverages] = 2,
			[RoleCash] = 1,
			[RoleTap] = 1,
			[RoleMuddiInCharge] = 1
		};
		var defaultFramework = new ShiftFramework(TimeSpan.FromMinutes(90), defaultRoles);
		var sixtyMinuteFramework = new ShiftFramework(TimeSpan.FromMinutes(60), defaultRoles);
		var container1 = new ShiftContainer(defaultFramework, DateTime.UtcNow.Date.AddHours(11), 6);
		var container2 = new ShiftContainer(sixtyMinuteFramework, DateTime.UtcNow.Date.AddHours(20), 6);
		ShiftPlaces[0].AddContainer(container1);
		ShiftPlaces[0].AddContainer(container2);
	}

	public static IEnumerable<ShiftLocation> GetAllShiftPlaces() => ShiftPlaces;
	public static ShiftLocation GetPlaceById(Guid id) => ShiftPlaces.SingleOrDefault(t => t.Id == id) ?? ShiftPlaces[0];
	public static Task<ShiftLocation> GetPlaceByIdAsync(Guid id) => Task.FromResult(GetPlaceById(id));
}