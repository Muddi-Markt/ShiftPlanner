using System.Net;
using Muddi.ShiftPlanner.Client.Entities;
using Muddi.ShiftPlanner.Shared;
using Muddi.ShiftPlanner.Shared.Api;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Requests;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses;
using Muddi.ShiftPlanner.Shared.Entities;

namespace Muddi.ShiftPlanner.Client.Services;

// public interface IShiftService
// {
// 	Task<IEnumerable<ShiftLocation>> GetAllShiftLocationsAsync();
// 	Task<ShiftLocation> GetLocationsByIdAsync(Guid id);
// 	Task<IEnumerable<Shift>> GetAllShiftsFromLocationAsync(Guid id);
// 	Task AddShiftToLocation(ShiftLocation location, Shift shift);
// }

public class ShiftService
{
	private readonly IMuddiShiftApi _shiftApi;

	public ShiftService(IMuddiShiftApi shiftApi)
	{
		_shiftApi = shiftApi;
	}

	public async Task<IEnumerable<ShiftLocation>> GetAllShiftLocationsAsync()
	{
		var dtos = await _shiftApi.GetAllLocations();
		return dtos.Select(t => t.MapToShiftLocation());
	}

	public async Task<ShiftLocation> GetLocationsByIdAsync(Guid id)
	{
		var dto = await _shiftApi.GetLocationById(id);

		return dto.MapToShiftLocation();
	}

	public async Task<IEnumerable<Shift>> GetAllShiftsFromLocationAsync(Guid id)
	{
		var dtos = await _shiftApi.GetAllShiftsForLocation(id);
		return dtos.Select(t => t.MapToShift());
	}

	public async Task AddShiftToLocation(ShiftLocation location, CreateShiftRequest req)
	{
		var res = await _shiftApi.CreateShiftForLocation(location.Id, req);
	}
	
	public async Task<Guid> AddShiftToContainer(Guid containerId, CreateShiftRequest req)
	{
		var res = await _shiftApi.CreateShiftForContainer(containerId, req);
		return res.Id;
	}

	public async Task<IEnumerable<ShiftType>> GetAvailableShiftTypesAtTime(Guid containerId, DateTime start)
	{
		var availableShiftTypes = await _shiftApi.GetAvailableShiftTypes(containerId, start);
		return availableShiftTypes.Select(q => q.MapToShiftType());
	}

	public async Task<Shift?> GetShiftById(Guid id)
	{
		var resp = await _shiftApi.GetShift(id);
		if (resp.StatusCode == HttpStatusCode.NotFound)
			return null;
		if (resp.Error is not null)
			throw resp.Error;
		return resp.Content!.MapToShift();
	}
}