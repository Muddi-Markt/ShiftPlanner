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
	[CascadingParameter] public MainLayout MainLayout { get; set; } = default!;

	protected override void OnInitialized()
	{
		MainLayout.SetTitle(null);
	}
}