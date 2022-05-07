using Microsoft.AspNetCore.Components;
using Muddi.ShiftPlanner.Shared.Api;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses;
using Radzen;

namespace Muddi.ShiftPlanner.Client.Pages.Admin;

public abstract class UpdateDialogBase<TResponse> : ComponentBase , IUpdateDialogBase
	where TResponse : IMuddiResponse, new()
{
	[Inject] protected IMuddiShiftApi ShiftApi { get; set; }
	[Inject] protected DialogService DialogService { get; set; }
	[Parameter] public TResponse EntityToEdit { get; set; } = new();

	protected abstract Task Create();
	protected abstract Task Update();
	
	protected async Task UpdateAndClose()
	{
		try
		{
			if (EntityToEdit.Id == default)
				await Create();
			else
				await Update();
			DialogService.Close(true);
		}
		catch (Exception ex)
		{
			await DialogService.Error(ex);
		}
	}
	protected void Abort()
	{
		DialogService.Close(false);
	}
}

public interface IUpdateDialogBase
{
}