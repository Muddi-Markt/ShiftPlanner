using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Entities;
using Muddi.ShiftPlanner.Shared.Contracts.v1;

namespace Muddi.ShiftPlanner.Server.Api.Extensions;

public static class ShiftLocationEntityExtensions
{
	public static IQueryable<ShiftLocationEntity> CheckAdminOnly(this IQueryable<ShiftLocationEntity> shiftLocations,
		ClaimsPrincipal user)
	{
		return user.IsInRole(ApiRoles.Admin)
			? shiftLocations
			: shiftLocations.Where(x => x.Type.AdminOnly == false);
	}
}