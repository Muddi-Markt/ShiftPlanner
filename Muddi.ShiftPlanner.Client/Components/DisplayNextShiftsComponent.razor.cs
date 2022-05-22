using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Muddi.ShiftPlanner.Client.Services;
using Muddi.ShiftPlanner.Shared.Entities;

namespace Muddi.ShiftPlanner.Client.Components;

public partial class DisplayNextShiftsComponent
{
	[Inject] private ShiftService ShiftService { get; set; }
	[CascadingParameter] private Task<AuthenticationState> AuthStateTask { get; set; }
	[CascadingParameter] private IEnumerable<ShiftLocation> Locations { get; set; }
	private List<Shift> _shifts = new();
	protected override async Task OnInitializedAsync()
	{
		var authState = await AuthStateTask;
		if (authState.User.Identity?.IsAuthenticated == true)
		{
			_shifts.Clear();
			_shifts.AddRange(await ShiftService.GetAllShiftsFromUser(authState.User, 6));
		}
	}
}