using Muddi.ShiftPlanner.Shared.Entities;

namespace Muddi.ShiftPlanner.Client.Entities
{
	public class TemplateFormShiftParameter
	{
		public TemplateFormShiftParameter(ShiftContainer container, MuddiConnectUser user, DateTime startTime)
		{
			Container = container;
			StartTime = startTime;
			User = user;
		}

		public TemplateFormShiftParameter(ShiftContainer container, MuddiConnectUser user, Shift shiftToEdit)
		{
			ShiftToEdit = shiftToEdit;
			User = user;
			Container = container;
			StartTime = ShiftToEdit.StartTime;
		}

		public Shift? ShiftToEdit { get; }
		public ShiftContainer Container { get; }
		public WorkingUserBase User { get; }
		public DateTime StartTime { get; set; }
		public ShiftRole? Role { get; set; }
	}
}