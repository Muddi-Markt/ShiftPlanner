using System.Reflection.Metadata;
using Refit;

namespace Muddi.ShiftPlanner.Shared.Api;

public interface IMuddiShiftApi
{
	#region Locations

	[Get("/locations")]
	Task<IReadOnlyCollection<GetLocationResponse>> LocationsGetAll();

	[Post("/locations")]
	Task LocationsCreate(CreateLocationRequest createLocationRequest);

	[Delete("/locations/{Id}")]
	Task DeleteLocationAsync(Guid id);

	#endregion

	#region location-types

	[Get("/location-types")]
	Task<IReadOnlyCollection<GetLocationTypesResponse>> LocationTypesGetAll();

	[Post("/location-types")]
	Task LocationTypesCreate(CreateLocationTypeRequest createLocationTypeRequest);

	#endregion

	#region shift-types

	[Get("/shift-types")]
	Task<IReadOnlyCollection<GetShiftTypesResponse>> ShiftTypesGetAll();

	[Post("/shift-types")]
	Task ShiftTypesCreate(CreateShiftTypeRequest createShiftTypeRequest);

	[Delete("/shift-types/{Id}")]
	Task DeleteShiftType(Guid id);

	[Put("/shift-types/{EntityToEdit.Id}")]
	Task ShiftTypesUpdate(GetShiftTypesResponse entityToEdit);

	#endregion

	[Get("/frameworks")]
	Task<IReadOnlyCollection<GetFrameworkResponse>> FrameworksGetAll();

	[Delete("/frameworks/{Id}")]
	Task DeleteFramework(Guid id);

	[Post("/frameworks")]
	Task FrameworksCreate(CreateFrameworkRequest createFrameworkRequest);

	[Put("/frameworks/{EntityToEdit.Id}")]
	Task FrameworksUpdate(GetFrameworkResponse entityToEdit);
}