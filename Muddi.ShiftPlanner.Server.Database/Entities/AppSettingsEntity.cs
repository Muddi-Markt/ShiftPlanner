using Muddi.ShiftPlanner.Shared.Entities;

namespace Muddi.ShiftPlanner.Server.Database.Entities;

/// <summary>
/// Single-row entity storing app-wide settings in a <c>jsonb</c> column.
/// </summary>
public class AppSettingsEntity
{
    public int Id { get; set; } = 1;

    /// <summary>
    /// App settings, persisted as a single <c>jsonb</c> column via EF Core's native JSON mapping.
    /// </summary>
    public ApplicationSettings Settings { get; set; } = new();
}
