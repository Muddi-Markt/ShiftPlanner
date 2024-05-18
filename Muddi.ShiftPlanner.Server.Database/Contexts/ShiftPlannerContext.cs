using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Entities;


namespace Muddi.ShiftPlanner.Server.Database.Contexts;

public class ShiftPlannerContext : DbContext
{
	public ShiftPlannerContext(DbContextOptions<ShiftPlannerContext> options) : base(options)
	{
	}

	public DbSet<SeasonEntity> Seasons { get; set; }
	public DbSet<ShiftEntity> Shifts { get; set; }
	public DbSet<ShiftContainerEntity> Containers { get; set; }
	public DbSet<ShiftFrameworkEntity> ShiftFrameworks { get; set; }
	public DbSet<ShiftTypeEntity> ShiftTypes { get; set; }
	public DbSet<ShiftLocationEntity> ShiftLocations { get; set; }
	public DbSet<ShiftLocationTypeEntity> ShiftLocationTypes { get; set; }


	private static readonly SemaphoreSlim SeasonCacheSemaphore = new(1, 1);
	private static readonly Dictionary<Guid, SeasonOfflineDataDbto> SeasonCache = new();
	private static DateTime LastCacheInvalidation { get; set; } = DateTime.UtcNow;


	public override int SaveChanges(bool acceptAllChangesOnSuccess)
	{
		InvalidateCache();
		return base.SaveChanges(acceptAllChangesOnSuccess);
	}

	public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
		CancellationToken cancellationToken = default)
	{
		InvalidateCache();
		return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
	}

	private void InvalidateCache()
	{
		if (!ChangeTracker.Entries().Any(e => e.State
			    is EntityState.Added
			    or EntityState.Modified
			    or EntityState.Deleted))
			return;

		SeasonCacheSemaphore.Wait();
		try
		{
			//TODO maybe find a way to only invalidate the season on which the entity was changed
			LastCacheInvalidation = DateTime.UtcNow;
			SeasonCache.Clear();
		}
		finally
		{
			SeasonCacheSemaphore.Release();
		}
	}


	public async Task<SeasonOfflineDataDbto> GetSeasonOfflineDataCache(Guid seasonId, CancellationToken ct)
	{
		await SeasonCacheSemaphore.WaitAsync(ct);
		try
		{
			if (SeasonCache.TryGetValue(seasonId, out var data))
				return data;
			data = await GetSeasonOfflineData(seasonId, ct);
			SeasonCache[seasonId] = data;
			return data;
		}
		finally
		{
			SeasonCacheSemaphore.Release();
		}
	}

	private async Task<SeasonOfflineDataDbto> GetSeasonOfflineData(Guid seasonId, CancellationToken ct)
	{
		if (SeasonCacheSemaphore.CurrentCount != 0)
			throw new InvalidOperationException("Semaphore must be awaited");
		var season = await Seasons.FindAsync([seasonId], cancellationToken: ct);
		ArgumentNullException.ThrowIfNull(season);

		var shifts = await Shifts
			.Where(s => s.Type.Season == season)
			.OrderBy(s => s.Start)
			.Select(ShiftDbto.FromEntity)
			.ToListAsync(cancellationToken: ct);
		var locations = await ShiftLocations
			.Where(q => q.Season == season)
			.Select(ShiftLocationDbto.FromEntity)
			.ToListAsync(cancellationToken: ct);
		var frameworks = await ShiftFrameworks
			.Where(sf => sf.Season == season)
			.Select(ShiftFrameworkDbto.FromEntity)
			.ToListAsync(cancellationToken: ct);
		var shiftTypes = await ShiftTypes
			.Where(x => x.Season == season)
			.Select(ShiftTypeDbto.FromEntity)
			.ToListAsync(ct);
		var containers = await Containers
			.Where(l => l.Location.Season == season)
			.Select(ContainerDbto.FromEntity)
			.ToListAsync(cancellationToken: ct);
		var shiftLocationTypes = await ShiftLocationTypes
			.Select(ShiftLocationTypeDbto.FromEntity)
			.ToListAsync(ct);

		return new()
		{
			CacheTime = LastCacheInvalidation,
			Season = SeasonDbto.FromEntity.Compile().Invoke(season),
			Shifts = shifts,
			Containers = containers,
			ShiftFrameworks = frameworks,
			ShiftTypes = shiftTypes,
			ShiftLocations = locations,
			ShiftLocationTypes = shiftLocationTypes
		};
	}

	public DateTime GetLastCacheTime() => LastCacheInvalidation;
}