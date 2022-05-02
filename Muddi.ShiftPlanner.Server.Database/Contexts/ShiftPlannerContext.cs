using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Database.Entities;

namespace Muddi.ShiftPlanner.Server.Database.Contexts;

public class ShiftPlannerContext : DbContext
{
	public ShiftPlannerContext(DbContextOptions<ShiftPlannerContext> options) : base(options)
	{
	}

	public DbSet<Shift> Shifts { get; set; }
	public DbSet<ShiftContainer> Containers { get; set; }
	public DbSet<ShiftFramework> ShiftFrameworks { get; set; }
	public DbSet<ShiftType> ShiftTypes { get; set; }
	public DbSet<ShiftLocation> ShiftLocations { get; set; }
	public DbSet<ShiftLocationType> ShiftLocationTypes { get; set; }
	
}