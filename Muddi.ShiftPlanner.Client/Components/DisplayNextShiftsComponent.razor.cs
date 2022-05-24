using System.Collections.Specialized;
using System.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Muddi.ShiftPlanner.Client.Entities;
using Muddi.ShiftPlanner.Client.Pages.Locations;
using Muddi.ShiftPlanner.Client.Services;
using Muddi.ShiftPlanner.Shared.Entities;

namespace Muddi.ShiftPlanner.Client.Components;

public partial class DisplayNextShiftsComponent
{
	[Inject] private ShiftService ShiftService { get; set; }
	[CascadingParameter] private Task<AuthenticationState> AuthStateTask { get; set; }
	[CascadingParameter] private IEnumerable<ShiftLocation> Locations { get; set; }
	private List<Appointment>? _myShifts;
	private List<Appointment>? _freeShifts;


	protected override async Task OnInitializedAsync()
	{
		var authState = await AuthStateTask;
		if (authState.User.Identity?.IsAuthenticated == true)
		{
			var shifts = await ShiftService.GetAllShiftsFromUser(authState.User, 6);
			_myShifts = new (shifts.Select(s => s.ToAppointment()));
			shifts = await ShiftService.GetAllAvailableShifts(6);
			_freeShifts = new(shifts.Select(s => s.ToAppointment()));
		}
	}

	private static string MakeLocationUri(Appointment appointment, bool showOnlyUserShift = true)
	{
		var query = HttpUtility.ParseQueryString(string.Empty);
		query[nameof(LocationPage.StartDate)] = appointment.LocalStartTime.ToString("yyyy-MM-dd");
		query[nameof(LocationPage.ShowOnlyUsersShifts)] = showOnlyUserShift.ToString();
		return $"/locations/{appointment.Shift!.LocationId}/?{query}";
	}
}