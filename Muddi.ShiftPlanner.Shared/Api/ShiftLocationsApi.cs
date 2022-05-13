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

	[Put("/frameworks/{EntityToEdit.Id}")]
	Task UpdateFramework(GetFrameworkResponse entityToEdit);

	[Post("/containers")]
	Task CreateContainer(CreateContainerRequest createContainerRequest);
	[Delete("/containers/{id}")]
	Task DeleteContainer(Guid id);
	
	
	[Get("/locations/{Id}/shifts")]
	Task<IEnumerable<GetShiftResponse>> GetAllShiftsForLocation(Guid id);

	[Post("/locations/{Id}/shifts")]
	Task CreateShift(Guid id, CreateLocationsShiftRequest createLocationsShiftRequest);
	
}