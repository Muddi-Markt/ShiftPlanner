using Muddi.ShiftPlanner.Shared;
using Muddi.ShiftPlanner.Shared.Api;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses;
using Muddi.ShiftPlanner.Shared.Entities;

namespace Muddi.ShiftPlanner.Client.Services;

public class ShiftService : IShiftService
{
	private readonly IMuddiShiftApi _shiftApi;

	public ShiftService(IMuddiShiftApi shiftApi)
	{
		_shiftApi = shiftApi;
	}

	public async Task<IEnumerable<ShiftLocation>> GetAllShiftLocationsAsync()
	{
		var dtos = await _shiftApi.ShiftLocationsGetAllAsync();
		return dtos.Select(t => t.MapToShiftLocation());
	}

	public async Task<ShiftLocation> GetLocationsByIdAsync(Guid id)
	{
		var dto = await _shiftApi.ShiftLocationsGetAsync(id);

		return dto.MapToShiftLocation();
	}

	public async Task<IEnumerable<Shift>> GetAllShiftsFromLocationAsync(Guid id)
	{
		var dtos = await _shiftApi.LocationsGetAllShifts(id);
		return dtos.Select(t => t.MapToShift());
	}
}