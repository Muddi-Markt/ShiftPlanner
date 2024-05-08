using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Muddi.ShiftPlanner.Server.Database.Contexts;

namespace Muddi.ShiftPlanner.Server.Database.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddMuddiShiftPlannerContext(this IServiceCollection services, IConfiguration configuration)
    {
		services.AddDbContext<ShiftPlannerContext>(options =>
            options.UseNpgsql(BuildConnectionString(configuration))
                   .UseSnakeCaseNamingConvention()
        );
    }

    public static string BuildConnectionString(IConfiguration configuration)
    {
        // Reading environment variables
        string user = Environment.GetEnvironmentVariable("POSTGRES_USER");
        string password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
        string database = Environment.GetEnvironmentVariable("POSTGRES_DB");
        string host = Environment.GetEnvironmentVariable("POSTGRES_HOST");
        string port = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432"; // Default port if not set

        // Check if all environment variables are set
        if (string.IsNullOrEmpty(user) ||
            string.IsNullOrEmpty(password) ||
            string.IsNullOrEmpty(database) ||
            string.IsNullOrEmpty(host))
        {
            // Fallback to a predefined connection string from configuration if any env vars are missing
            return configuration.GetConnectionString("ShiftPlannerDb");
        }

        // Building the connection string
        return $"Host={host};Port={port};Database={database};Username={user};Password={password}";
    }
}