using System.Linq.Expressions;

namespace Muddi.ShiftPlanner.Server.Database.Entities;

public interface IDatabaseTransferObject<TEntity, TSelf>
	where TSelf : IDatabaseTransferObject<TEntity, TSelf>
{
	static abstract Expression<Func<TEntity, TSelf>> FromEntity { get; }
}

public class SeasonOfflineDataDbto
{
	public required DateTime CacheTime { get; init; }
	public required SeasonDbto Season { get; init; }
	public required List<ShiftDbto> Shifts { get; init; }
	public required List<ContainerDbto> Containers { get; init; }
	public required List<ShiftFrameworkDbto> ShiftFrameworks { get; init; }
	public required List<ShiftTypeDbto> ShiftTypes { get; init; }
	public required List<ShiftLocationDbto> ShiftLocations { get; init; }
	public required List<ShiftLocationTypeDbto> ShiftLocationTypes { get; init; }
}

public class ShiftDbto : IDatabaseTransferObject<ShiftEntity, ShiftDbto>
{
	public required Guid Id { get; init; }
	public required Guid EmployeeKeycloakId { get; init; }
	public required Guid ShiftContainer { get; init; }
	public required DateTime Start { get; init; }
	public required DateTime End { get; init; }
	public required Guid ShiftType { get; init; }

	public static Expression<Func<ShiftEntity, ShiftDbto>> FromEntity => entity => new()
	{
		Id = entity.Id,
		EmployeeKeycloakId = entity.EmployeeKeycloakId,
		ShiftContainer = entity.ShiftContainer.Id,
		Start = entity.Start,
		End = entity.End,
		ShiftType = entity.Type.Id
	};
}

public class ShiftLocationDbto : IDatabaseTransferObject<ShiftLocationEntity, ShiftLocationDbto>
{
	public required Guid Id { get; set; }
	public required string Name { get; set; }
	public required Guid Type { get; set; }


	public static Expression<Func<ShiftLocationEntity, ShiftLocationDbto>> FromEntity => entity => new ShiftLocationDbto
	{
		Id = entity.Id,
		Name = entity.Name,
		Type = entity.Type.Id
	};
}

public class ShiftFrameworkDbto : IDatabaseTransferObject<ShiftFrameworkEntity, ShiftFrameworkDbto>
{
	public required Guid Id { get; set; }
	public required string Name { get; set; }
	public required int SecondsPerShift { get; set; }

	public static Expression<Func<ShiftFrameworkEntity, ShiftFrameworkDbto>> FromEntity => entity => new()
	{
		Id = entity.Id,
		Name = entity.Name,
		SecondsPerShift = entity.SecondsPerShift
	};
}

public class ShiftTypeDbto : IDatabaseTransferObject<ShiftTypeEntity, ShiftTypeDbto>
{
	public required Guid Id { get; set; }
	public required string Name { get; set; }
	public required string Color { get; set; }
	public required TimeSpan StartingTimeShift { get; set; }
	public required bool OnlyAssignableByAdmin { get; set; }
	public required string? Description { get; set; }

	public static Expression<Func<ShiftTypeEntity, ShiftTypeDbto>> FromEntity => entity => new()
	{
		Id = entity.Id,
		Name = entity.Name,
		Color = entity.Color,
		StartingTimeShift = entity.StartingTimeShift,
		OnlyAssignableByAdmin = entity.OnlyAssignableByAdmin,
		Description = entity.Description
	};
}

public class ContainerDbto : IDatabaseTransferObject<ShiftContainerEntity, ContainerDbto>
{
	public required Guid Id { get; set; }
	public required DateTime Start { get; set; }
	public required DateTime End { get; set; }
	public required int TotalShifts { get; set; }
	public required string Color { get; set; }
	public required Guid Location { get; set; }
	public required Guid Framework { get; set; }

	public static Expression<Func<ShiftContainerEntity, ContainerDbto>> FromEntity => entity => new ContainerDbto
	{
		Id = entity.Id,
		Start = entity.Start,
		End = entity.End,
		TotalShifts = entity.TotalShifts,
		Color = entity.Color,
		Location = entity.Location.Id,
		Framework = entity.Framework.Id
	};
}

public class ShiftLocationTypeDbto : IDatabaseTransferObject<ShiftLocationTypeEntity, ShiftLocationTypeDbto>
{
	public required Guid Id { get; init; }
	public required string Name { get; init; }

	public static Expression<Func<ShiftLocationTypeEntity, ShiftLocationTypeDbto>> FromEntity => entity => new()
	{
		Id = entity.Id,
		Name = entity.Name
	};
}

public class SeasonDbto : IDatabaseTransferObject<SeasonEntity, SeasonDbto>
{
	public required Guid Id { get; init; }
	public required string Name { get; init; }
	public required DateTime StartDate { get; init; }
	public required DateTime EndDate { get; init; }

	public static Expression<Func<SeasonEntity, SeasonDbto>> FromEntity => entity => new()
	{
		Id = entity.Id,
		Name = entity.Name,
		StartDate = entity.StartDate,
		EndDate = entity.EndDate
	};
}