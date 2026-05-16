using Microsoft.AspNetCore.Components;
using Muddi.ShiftPlanner.Client.Shared;

namespace Muddi.ShiftPlanner.Client.Pages;

public partial class Index
{
	[CascadingParameter] public MainLayout MainLayout { get; set; } = default!;

	protected override void OnInitialized()
	{
		MainLayout.SetTitle(null);
	}
}