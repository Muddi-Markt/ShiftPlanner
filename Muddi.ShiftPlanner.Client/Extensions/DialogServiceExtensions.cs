using Radzen;

namespace Muddi.ShiftPlanner.Client;

public static class DialogServiceExtensions
{
	public static Task Error(this DialogService dialogService, Exception ex, string? title = null)
	{
		return dialogService.Error(ex.Message + "\r\n" + ex.InnerException?.Message, title ?? $"Error - {ex.GetType().Name}");
	}
	public static Task Error(this DialogService dialogService, string errorText, string title = "Error")
	{
		return dialogService.Confirm(errorText, title);
	}
}