using System.Reflection;
using System.Reflection.Metadata;
using Muddi.ShiftPlanner.Shared.Entities;
using Refit;

namespace Muddi.ShiftPlanner.Shared.Api;

public interface IMuddiShiftApi
{
	#region Locations

	[Get("/locations/{Id}")]
	Task<GetLocationResponse> GetLocationById(Guid id);

	[Get("/locations")]
	Task<IReadOnlyCollection<GetLocationResponse>> GetAllLocations();

	[Post("/locations")]
	Task CreateLocation(CreateLocationRequest createLocationRequest);

	[Delete("/locations/{Id}")]
	Task DeleteLocationAsync(Guid id);

	#endregion

	#region location-types

	[Get("/location-types")]
	Task<IReadOnlyCollection<GetLocationTypesResponse>> GetAllLocationTypes();

	[Post("/location-types")]
	Task CreateLocationType(CreateLocationTypeRequest createLocationTypeRequest);

	#endregion

	#region shift-types

	[Get("/shift-types")]
	Task<IReadOnlyCollection<GetShiftTypesResponse>> GetAllShiftTypes();

	[Post("/shift-types")]
	Task CreateShiftType(CreateShiftTypeRequest createShiftTypeRequest);

	[Delete("/shift-types/{Id}")]
	Task DeleteShiftType(Guid id);

	[Put("/shift-types/{EntityToEdit.Id}")]
	Task UpdateShiftType(GetShiftTypesResponse entityToEdit);

	#endregion

	[Get("/frameworks")]
	Task<IReadOnlyCollection<GetFrameworkResponse>> GetAllFrameworks();

	[Delete("/frameworks/{Id}")]
	Task DeleteFramework(Guid id);

	[Post("/frameworks")]
	Task CreateFramework(CreateFrameworkRequest createFrameworkRequest);

	[Put("/frameworks/{id}")]
	Task UpdateFramework(Guid id, UpdateFrameworkRequest entityToEdit);

	[Post("/containers")]
	Task CreateContainer(CreateContainerRequest createContainerRequest);

	[Delete("/containers/{id}")]
	Task DeleteContainer(Guid id);


	[Get("/locations/{Id}/shifts")]
	Task<IEnumerable<GetShiftResponse>> GetAllShiftsForLocation(Guid id, GetAllShiftsForLocationRequest request);

	[Post("/locations/{Id}/shifts")]
	Task<IApiResponse> CreateShiftForLocation(Guid id, CreateShiftRequest createShiftRequest);

	[Get("/containers/{containerId}/get-available-shift-types")]
	Task<IEnumerable<GetShiftTypesCountResponse>> GetAvailableShiftTypes(Guid containerId, DateTime start);

	[Post("/containers/{Id}/shifts")]
	Task<DefaultCreateResponse> CreateShiftForContainer(Guid id, CreateShiftRequest createShiftRequest);

	[Delete("/shifts/{id}")]
	Task DeleteShift(Guid id);

	[Get("/shifts/{id}")]
	Task<ApiResponse<GetShiftResponse>> GetShift(Guid id);
	
	[Get("/shifts/available-types")]
	Task<ICollection<GetShiftTypesCountResponse>> GetAvailableShiftTypes(GetAllShiftsRequest request);
	

	[Put("/shifts/{id}")]
	Task UpdateShift(Guid id, CreateShiftRequest request);

	[Get("/containers/{id}")]
	Task<GetContainerResponse> GetContainer(Guid id);

	[Put("/locations/{id}")]
	Task UpdateLocation(Guid id, UpdateLocationRequest updateLocationRequest);

	[Get("/employees/{keycloakId}/shifts")]
	Task<IEnumerable<GetShiftResponse>> GetAllShiftsFromEmployee(Guid keycloakId,int count);

	[Get("/locations/{Id}/get-all-available-shifts-types")]
	Task<IEnumerable<GetShiftTypesCountResponse>> GetAllAvailableShiftTypesFromLocationAsync(Guid id, GetAvailableShiftsForLocationRequest request);

	[Get("/employees")]
	Task<IEnumerable<GetEmployeeResponse>> GetAllEmployees();

	[Get("/locations/get-all-shifts-count")]
	Task<IEnumerable<GetShiftsCountResponse>> GetAllLocationShiftsCount();

	[Get("/locations/{locationId}/get-shifts-count")]
	Task<GetShiftsCountResponse> GetLocationShiftsCount(Guid locationId);
}

public class CustomUrlParameterFormatter : Refit.DefaultUrlParameterFormatter
{
	public override string? Format(object? parameterValue, ICustomAttributeProvider attributeProvider, Type type)
	{
		return parameterValue is DateTime dt
			? dt.ToUniversalTime().ToString("O")
			: base.Format(parameterValue, attributeProvider, type);
	}
}