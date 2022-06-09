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
		shiftResponse.Employee = keycloakUser;//TODO For backwards compatibility, delete this in future!! 
		shiftResponse.EmployeeId = keycloakUser.Id;
		shiftResponse.EmployeeFullName = keycloakUser.FullName;

		return shiftResponse;
	}
	
	public static GetShiftResponse MapToShiftResponse(this ShiftEntity shift, Guid employeeId, string? employeeFullName = null)
	{
		var shiftResponse = shift.Adapt<GetShiftResponse>();
		shiftResponse.ContainerId = shift.ShiftContainer.Id;
		shiftResponse.LocationId = shift.ShiftContainer.Location.Id;
		shiftResponse.EmployeeId = employeeId;
		shiftResponse.EmployeeFullName = employeeFullName;

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