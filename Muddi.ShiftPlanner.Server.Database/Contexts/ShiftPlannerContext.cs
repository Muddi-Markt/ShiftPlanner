using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Entities;

namespace Muddi.ShiftPlanner.Server.Database.Contexts;

public class ShiftPlannerContext : DbContext
{
	public ShiftPlannerContext(DbContextOptions<ShiftPlannerContext> options) : base(options)
	{
	}

	public DbSet<ShiftEntity> Shifts { get; set; }
	public DbSet<ShiftContainerEntity> Containers { get; set; }
	public DbSet<ShiftFrameworkEntity> ShiftFrameworks { get; set; }
	public DbSet<ShiftTypeEntity> ShiftTypes { get; set; }
	public DbSet<ShiftLocationEntity> ShiftLocations { get; set; }
	public DbSet<ShiftLocationTypeEntity> ShiftLocationTypes { get; set; }
	
}