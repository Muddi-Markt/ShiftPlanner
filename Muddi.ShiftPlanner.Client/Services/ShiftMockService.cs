using Muddi.ShiftPlanner.Shared;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses;
using Muddi.ShiftPlanner.Shared.Entities;

namespace Muddi.ShiftPlanner.Client.Services;

internal class TestUser : EmployeeBase
{
	public TestUser(string name) : base(Guid.NewGuid(), name)
	{
	}
}

public interface IShiftService
{
	Task<IEnumerable<ShiftLocation>> GetAllShiftLocationsAsync();
	Task<ShiftLocation> GetLocationsByIdAsync(Guid id);
	Task<IEnumerable<Shift>> GetAllShiftsFromLocationAsync(Guid id);
}