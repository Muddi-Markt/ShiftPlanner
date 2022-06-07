using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Muddi.ShiftPlanner.Client.Services;
using Muddi.ShiftPlanner.Client.Shared;
using Muddi.ShiftPlanner.Shared;
using Muddi.ShiftPlanner.Shared.Entities;

namespace Muddi.ShiftPlanner.Client.Pages;

public partial class Index
{
	[Inject] private IAccessTokenProvider AccessTokenProvider { get; set; }
	[Inject] private NavigationManager NavigationManager { get; set; }
	[CascadingParameter] public MainLayout MainLayout { get; set; } = default!;

	private string _token;


	protected override async Task OnInitializedAsync()
	{
		MainLayout.SetTitle("Start");
		var t = await AccessTokenProvider.RequestAccessToken();
		_token = t.TryGetToken(out var tok)
			? tok.Value
			: $"Failed to obtain token. Status: {t.Status}";
	}
}