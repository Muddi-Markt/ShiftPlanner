namespace Muddi.ShiftPlanner.Shared.Entities;

public record Shift(WorkingUserBase User, DateTime StartTime, ShiftRole Role);