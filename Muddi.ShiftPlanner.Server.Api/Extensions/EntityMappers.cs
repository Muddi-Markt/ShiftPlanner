using Mapster;
using Muddi.ShiftPlanner.Server.Api.Services;
using Muddi.ShiftPlanner.Server.Database.Entities;

namespace Muddi.ShiftPlanner.Server.Api.Extensions;

public static class EntityMappers
{
	public static GetShiftResponse MapToShiftResponse(this ShiftEntity shift, GetEmployeeResponse keycloakUser)
	{
		var shiftResponse = shift.Adapt<GetShiftResponse>();
		shiftResponse.ContainerId = shift.ShiftContainer.Id;
		shiftResponse.LocationId = shift.ShiftContainer.Location.Id;
		shiftResponse.Employee = keycloakUser;

		return shiftResponse;
	}

	public static GetEmployeeResponse MapToEmployeeResponse(this KeycloakUserRepresentation keycloakUser)
	{
		return new GetEmployeeResponse
		{
			Email = keycloakUser!.Email,
			Id = Guid.Parse(keycloakUser.Id ?? throw new InvalidOperationException("Keycloak Id is null")),
			UserName = keycloakUser.Username,
			FirstName = keycloakUser.FirstName,
			LastName = keycloakUser.LastName
		};
	}

}