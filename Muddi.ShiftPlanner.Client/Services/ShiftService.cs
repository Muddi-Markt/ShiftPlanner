using System.Net;
using System.Security.Claims;
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
	public Task InitializedTask => _initializedTsc.Task;
	private readonly TaskCompletionSource _initializedTsc = new();
	private SemaphoreSlim _initalizeLock = new(1,1); 

	public ShiftService(IMuddiShiftApi shiftApi)
	{
		_shiftApi = shiftApi;
	}

	private IEnumerable<ShiftLocation>? _shiftLocations;

	public async Task Initialize()
	{

		await _initalizeLock.WaitAsync();
		if (_initializedTsc.Task.IsCompleted)
			return;
		try
		{
			var dtos = await _shiftApi.GetAllLocations();
			var counts = await _shiftApi.GetAllLocationShiftsCount();
			_shiftLocations = dtos.Join(counts, d => d.Id, c => c.Id, (d, c) => d.MapToShiftLocation(c));
			_initializedTsc.SetResult();
		}
		finally
		{
			_initalizeLock.Release();
		}
	}

	public IEnumerable<ShiftLocation> GetAllShiftLocations()
	{
		return _shiftLocations ?? Enumerable.Empty<ShiftLocation>();
	}

	public ShiftLocation? FindLocationById(Guid id)
	{
		return _shiftLocations?.First(l => l.Id == id);
	}

	public async Task<IEnumerable<GetShiftTypesCountResponse>> GetAllAvailableShiftTypesFromLocationAsync(Guid id, DateTime? start = null,
		DateTime? end = null)
	{
		var dtos = await _shiftApi.GetAllAvailableShiftTypesFromLocationAsync(id, new GetAvailableShiftsForLocationRequest
		{
			LocationId = id,
			StartTime = start,
			EndTime = end
		});
		return dtos;
	}

	public async Task<IEnumerable<Shift>> GetAllShiftsFromLocationAsync(Guid id, DateTime start, DateTime end, Guid? forEmployeeId = null)
	{
		var dtos = await _shiftApi.GetAllShiftsForLocation(id, new() { Start = start, End = end, KeycloakEmployeeId = forEmployeeId });
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

	public async Task<IEnumerable<GetShiftTypesResponse>> GetAvailableShiftTypesAtTime(Guid containerId, DateTime start)
	{
		var availableShiftTypes = await _shiftApi.GetAvailableShiftTypes(containerId, start);
		return availableShiftTypes.Where(q => q.AvailableCount > 0).Select(q => q.Type);
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


	public async Task<IEnumerable<Shift>> GetAllAvailableShifts(int limit, DateTime? startingFrom = null)
	{
		var types = await _shiftApi.GetAvailableShiftTypes(new() { Limit = limit, StartingFrom = startingFrom });
		return types.Select(t => t.MapToShift());
	}

	public async Task<IEnumerable<Shift>> GetAllShiftsFromUser(ClaimsPrincipal user, int count = -1, DateTime? startingFrom = null)
	{
		var shifts = await _shiftApi.GetAllShiftsFromEmployee(user.GetKeycloakId(), count, startingFrom);
		return shifts.Select(s => s.MapToShift());
	}

	public static void FillShiftsWithUnassignedShifts(ref List<Shift> shifts, IEnumerable<ShiftContainer> containers, DateTime startTime,
		DateTime endTime)
	{
		startTime = startTime.ToUniversalTime();
		endTime = endTime.ToUniversalTime();

		foreach (var container in containers)
		{
			foreach (var containerStart in container.ShiftStartTimes)
			{
				if (containerStart < startTime || containerStart >= endTime)
					continue;
				foreach (var (type, count) in container.Framework.RolesCount)
				{
					var assignedShiftsCount = shifts.Count(s =>
						s.ContainerId == container.Id
						&& s.Type == type
						&& s.StartTime == containerStart);
					if (assignedShiftsCount < count)
					{
						for (int i = 0; i < count - assignedShiftsCount; i++)
						{
							shifts.Add(new Shift(Mappers.NotAssignedEmployee, containerStart,
								containerStart + container.Framework.TimePerShift,
								type));
						}
					}
				}
			}
		}
	}

	public async Task<byte[]> GetAllShiftsFromUserAsICal()
	{
		var res = await _shiftApi.GetAllShiftsFromEmployeeAsIcsFile();
		return await res.ReadAsByteArrayAsync();
	}

	public async Task<ShiftLocation?> GetLocationsByIdAsync(Guid id)
	{
		var dto = await _shiftApi.GetLocationById(id);
		var count = await _shiftApi.GetLocationShiftsCount(id);

		return dto.MapToShiftLocation(count);
	}
}