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
	public DbSet<AppSettingsEntity> AppSettings { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// Single-row table; the whole settings object lives in one jsonb column (EF Core JSON mapping).
		modelBuilder.Entity<AppSettingsEntity>(entity =>
		{
			entity.HasKey(e => e.Id);
			entity.Property(e => e.Id).ValueGeneratedNever();
			entity.OwnsOne(e => e.Settings, owned => owned.ToJson());
			entity.Navigation(e => e.Settings).IsRequired();
		});
	}
}