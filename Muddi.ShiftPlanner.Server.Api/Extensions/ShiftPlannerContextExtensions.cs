using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Server.Database.Entities;

namespace Muddi.ShiftPlanner.Server.Api.Extensions;

public static class ShiftPlannerContextExtensions
{
	public static ShiftEntity AddShiftToContainer(this ShiftPlannerContext database, CreateShiftRequest req, ShiftContainerEntity container)
	{
		var endTime = req.Start + container.Framework.TimePerShift;
		var type = container.Framework.ShiftTypeCounts.Single(stc => stc.ShiftType.Id == req.ShiftTypeId).ShiftType;

		var shift = new ShiftEntity
		{
			Id = Guid.NewGuid(),
			EmployeeKeycloakId = req.EmployeeKeycloakId,
			Start = req.Start,
			End = endTime,
			Type = type
		};


		container.Shifts.Add(shift);
		database.Shifts.Add(shift);
		return shift;
	}
}