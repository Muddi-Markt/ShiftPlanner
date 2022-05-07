using Radzen;

namespace Muddi.ShiftPlanner.Client;

public static class DialogServiceExtensions
{
	public static Task Error(this DialogService dialogService, Exception ex)
	{
		return dialogService.Confirm(ex.Message, "Error");
	}
}