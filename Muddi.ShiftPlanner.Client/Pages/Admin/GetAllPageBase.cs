using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Muddi.ShiftPlanner.Shared.Api;
using Muddi.ShiftPlanner.Shared.BlazorWASM;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses;
using Radzen;
using Radzen.Blazor;

namespace Muddi.ShiftPlanner.Client.Pages.Admin;

[Authorize(Policy = Policies.IsAdmin)]
public abstract class GetAllPageBase<TResponse, TCreateDialog> : ComponentBase
	where TResponse : IMuddiResponse, new()
	where TCreateDialog : UpdateDialogBase<TResponse>

{
	[Inject] protected IMuddiShiftApi ShiftApi { get; set; }
	[Inject] protected DialogService DialogService { get; set; }
	protected IEnumerable<TResponse> Data { get; set; } = Enumerable.Empty<TResponse>();
	protected bool IsLoading { get; private set; }
	protected int Count { get; private set; }
	protected RadzenDataGrid<TResponse>? DataGrid { get; set; }

	protected abstract string NameOfEntity { get; }
	protected abstract Task<IReadOnlyCollection<TResponse>> LoadData();
	protected abstract Task Delete(TResponse request);

	protected override Task OnInitializedAsync()
	{
		return LoadGridData();
	}

	protected async Task LoadGridData()
	{
		IsLoading = true;
		try
		{
			var res = await LoadData();
			Count = res.Count;
			Data = res;
		}
		catch (Exception ex)
		{
			await DialogService.Confirm(ex.Message, "Failed to fetch data");
		}
		finally
		{
			IsLoading = false;
		}
	}

	protected async Task Create()
	{
		if (await DialogService.OpenAsync<TCreateDialog>($"Erstelle '{NameOfEntity}'") is true)
		{
			ReloadData();
		}
	}

	protected async Task Edit(TResponse entity)
	{
		if (await DialogService.OpenAsync<TCreateDialog>($"Bearbeite '{NameOfEntity}'", new()
		    {
			    [nameof(UpdateDialogBase<TResponse>.EntityToEdit)] = entity
		    }) is true)

		{
			ReloadData();
		}
	}

	protected async Task DeleteItem(TResponse entity, string? name = null)
	{
		if (await DialogService.Confirm($"Are you sure that you want delete '{name ?? "this"}'? This can't be undone.") is true)
		{
			try
			{
				await Delete(entity);
				ReloadData();
			}
			catch (Refit.ApiException ex)
			{
				await DialogService.Confirm(ex.Message);
			}
			catch (HttpRequestException ex)
			{
				await DialogService.Confirm($"{ex.Message}\r\nErrorCode: {ex.StatusCode}");
			}
		}
	}

	protected void ReloadData() => DataGrid?.Reload();
}