using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Muddi.ShiftPlanner.Client.Services;
using Muddi.ShiftPlanner.Shared;
using Muddi.ShiftPlanner.Shared.Entities;

namespace Muddi.ShiftPlanner.Client.Pages;

public partial class Index
{
	[Inject] private IAccessTokenProvider AccessTokenProvider { get; set; }
	[Inject] private NavigationManager NavigationManager { get; set; }
	
	private string _token;
	

	protected override async Task OnInitializedAsync()
	{
		var t = await AccessTokenProvider.RequestAccessToken();
		if (t.TryGetToken(out var tok))
		{
			_token = tok.Value;
		}
		else
		{
			_token = $"Failed to obtain token. Status: {t.Status}";
			if (t.Status == AccessTokenResultStatus.RequiresRedirect)
				NavigationManager.NavigateTo(t.RedirectUrl);
		}
	}
}