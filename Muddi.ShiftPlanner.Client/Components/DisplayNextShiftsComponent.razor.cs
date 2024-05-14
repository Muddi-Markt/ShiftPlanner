using System.Collections.Specialized;
using System.Web;
using Blazor.DownloadFileFast.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Muddi.ShiftPlanner.Client.Entities;
using Muddi.ShiftPlanner.Client.Pages.Locations;
using Muddi.ShiftPlanner.Client.Services;
using Muddi.ShiftPlanner.Client.Shared;
using Muddi.ShiftPlanner.Shared.Entities;
using Radzen;

namespace Muddi.ShiftPlanner.Client.Components;

public partial class DisplayNextShiftsComponent : IDisposable
{
	[Inject] private ShiftService ShiftService { get; set; } = default!;
	[Inject] private DialogService DialogService { get; set; } = default!;
	[Inject] public IBlazorDownloadFileService BlazorDownloadFileService { get; set; }
	[CascadingParameter] private Task<AuthenticationState> AuthStateTask { get; set; }
	private List<DayAppointment>? _myShifts;
	private List<DayAppointment>? _freeShifts;

	protected override async Task OnInitializedAsync()
	{
		await ShiftService.InitializedTask;
		ShiftService.OnSeasonChanged += Init;
		await Init();
	}

	private async Task Init()
	{
		var authState = await AuthStateTask;
		if (authState.User.Identity?.IsAuthenticated == true)
		{
			var shifts = await ShiftService.GetAllShiftsFromUser(authState.User, 6, DateTime.UtcNow);
			_myShifts = new(shifts.Select(s => s.ToAppointment()));
			var availableShifts = await ShiftService.GetAllAvailableShifts(12, DateTime.UtcNow);
			_freeShifts = [..availableShifts.Select(s => s.ToAppointment())];
			await InvokeAsync(StateHasChanged);
		}
	}

	private static string MakeLocationUri(DayAppointment appointment, bool showOnlyUserShift = true)
	{
		var query = HttpUtility.ParseQueryString(string.Empty);
		query[nameof(LocationPage.StartDate)] = appointment.LocalStartTime.ToString("yyyy-MM-dd");
		query[nameof(LocationPage.ShowOnlyUsersShifts)] = showOnlyUserShift.ToString();
		return $"/locations/{appointment.Shift!.LocationId}/?{query}";
	}

	private async Task Download_iCal()
	{
		var bytes = await ShiftService.GetAllShiftsFromUserAsICal(ShiftService.CurrentSeason.Id);
		if (!bytes.Any())
		{
			await DialogService.Alert("Du hast leider noch keine Schichten");
			return;
		}

		await BlazorDownloadFileService.DownloadFileAsync("muddi-calendar.ics", bytes);
	}

	public void Dispose()
	{
		ShiftService.OnSeasonChanged -= Init;
	}
}