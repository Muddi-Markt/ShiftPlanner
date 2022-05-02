using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Database.Extensions;

public static class ServiceCollectionExtensions
{
	
	public static void AddMuddiShiftPlannerContext(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddNpgsql<ShiftPlannerContext>(configuration.GetConnectionString("ShiftPlannerDb"), null,
			a => a.UseSnakeCaseNamingConvention());
	}
}