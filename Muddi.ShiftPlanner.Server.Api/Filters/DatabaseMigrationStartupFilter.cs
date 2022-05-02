using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Muddi.ShiftPlanner.Server.Api.Filters;

public class DatabaseMigrationStartupFilter<TContext> : IStartupFilter where TContext : DbContext
{
	private readonly ILogger<DatabaseMigrationStartupFilter<TContext>> _logger;

	public DatabaseMigrationStartupFilter(ILogger<DatabaseMigrationStartupFilter<TContext>> logger)
	{
		_logger = logger;
	}

	public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
	{
		return app =>
		{
			using (var scope = app.ApplicationServices.CreateScope())
			{
				foreach (var context in scope.ServiceProvider.GetServices<TContext>())
				{
					var pending = context.Database.GetPendingMigrations().ToArray();
					if (!pending.Any())
					{
						_logger.LogDebug("Found 0 migrations for {Context}", context.GetType().Name);
						continue;
					}

					_logger.LogInformation("Found {Count} migrations for {Context} Apply...", pending.Length, context.GetType().Name);

					context.Database.Migrate();
					_logger.LogInformation("Successfully migrated {Count} migration(s)", pending.Length);
				}
			}

			next(app);
		};
	}
}