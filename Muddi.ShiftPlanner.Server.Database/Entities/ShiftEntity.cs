using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Muddi.ShiftPlanner.Shared;

namespace Muddi.ShiftPlanner.Server.Database.Entities;

public class ShiftEntity
{
	public Guid Id { get; set; }
	public Guid EmployeeKeycloakId { get; set; }
	public required ShiftContainerEntity ShiftContainer { get; set; }
	public DateTime Start { get; set; }
	public DateTime End { get; set; }
	public required ShiftTypeEntity Type { get; set; }
	[MaxLength(50)]
	public string? BlockReason { get; set; }

	/// <summary>
	/// Returns true when this shift is blocked (no real employee assigned).
	/// </summary>
	[NotMapped]
	public bool IsBlocked => EmployeeKeycloakId == Mappers.NotAssignedEmployee.KeycloakId
	                         && !string.IsNullOrEmpty(BlockReason);
}