using System.Data.Common;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Npgsql;

namespace Muddi.ShiftPlanner.Server.Database.Extensions;

public static class ServiceCollectionExtensions
{
	public static void AddMuddiShiftPlannerContext(this IServiceCollection services, IConfiguration configuration)
	{
		if (configuration.GetConnectionString("ShiftPlannerDb") is not string connectionString)
		{
			var applicationName = Assembly.GetEntryAssembly()!.GetName().Name;
			var dbConfig = configuration.GetRequiredSection("Database");
			var builder = new NpgsqlConnectionStringBuilder
			{
				BrowsableConnectionString = false,
				ConnectionString = null,
				Host = dbConfig["Host"],
				Port = dbConfig.GetValue("Port", 5432),
				Database = dbConfig["Name"],
				Username = dbConfig["User"],
				Password = dbConfig["Password"],
				ApplicationName = applicationName,
				IncludeErrorDetail = dbConfig.GetValue("IncludeErrorDetail", false)
			};
			connectionString = builder.ConnectionString;
		}

		services.AddNpgsql<ShiftPlannerContext>(
			connectionString,
			null,
			a => a.UseSnakeCaseNamingConvention());
	}
}