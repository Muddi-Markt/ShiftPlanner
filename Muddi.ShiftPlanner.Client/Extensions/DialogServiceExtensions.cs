using Muddi.ShiftPlanner.Client.Components;
using Radzen;
using Refit;

namespace Muddi.ShiftPlanner.Client;

public static class DialogServiceExtensions
{
	public static Task Error(this DialogService dialogService, Exception ex, string? title = null)
	{
		var message = ex switch
		{
			ApiException apiException => $"{apiException.Message}\r\n{apiException.Content}",
			_ => ex.Message + "\r\n" + ex.InnerException?.Message
		};
		
		return dialogService.Error(message, title ?? $"Error - {ex.GetType().Name}");
	}
	public static Task Error(this DialogService dialogService, string errorText, string title = "Error")
	{
		return dialogService.OpenAsync<ErrorDialog>(title, new Dictionary<string, object>(){[nameof(ErrorDialog.Text)] = errorText});
	}
}