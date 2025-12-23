using System.Security.Claims;
using Mapster;
using Muddi.ShiftPlanner.Server.Api.Services;
using Muddi.ShiftPlanner.Server.Database.Entities;

namespace Muddi.ShiftPlanner.Server.Api.Extensions;

public static class EntityMappers
{
	public static GetShiftResponse MapToShiftResponse(this ShiftEntity shift, GetEmployeeResponse user)
	{
		var shiftResponse = shift.Adapt<GetShiftResponse>();
		shiftResponse.ContainerId = shift.ShiftContainer.Id;
		shiftResponse.LocationId = shift.ShiftContainer.Location.Id;
		shiftResponse.EmployeeId = user.Id;
		shiftResponse.EmployeeFullName = user.FullName;

		return shiftResponse;
	}

	public static GetShiftResponse MapToShiftResponse(this ShiftEntity shift, Guid employeeId,
		string? employeeFullName = null)
	{
		var shiftResponse = shift.Adapt<GetShiftResponse>();
		shiftResponse.ContainerId = shift.ShiftContainer.Id;
		shiftResponse.LocationId = shift.ShiftContainer.Location.Id;
		shiftResponse.EmployeeId = employeeId;
		shiftResponse.EmployeeFullName = employeeFullName;

		return shiftResponse;
	}

	public static GetEmployeeResponse MapToEmployeeResponse(this KeycloakUserRepresentation keycloakUser,
		ClaimsPrincipal user) => keycloakUser.MapToEmployeeResponse(user.IsInRole(ApiRoles.Admin));

	public static GetEmployeeResponse MapToEmployeeResponse(this KeycloakUserRepresentation keycloakUser,
		bool showLastNameFull)
	{
		return new GetEmployeeResponse
		{
			Id = keycloakUser.Id ?? throw new InvalidOperationException("Keycloak Id is null"),
			FirstName = keycloakUser.FirstName,
			LastName = keycloakUser.LastName is null || showLastNameFull
				? keycloakUser.LastName
				: keycloakUser.LastName[0].ToString() //If not admin, only print the first char of the last name
		};
	}
}