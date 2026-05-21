using System.Net;
using System.Security.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Muddi.ShiftPlanner.Shared;
using Muddi.ShiftPlanner.Shared.Contracts.v1;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Requests;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses;
using Muddi.ShiftPlanner.Shared.Entities;
using Radzen;
using Refit;

namespace Muddi.ShiftPlanner.Client.Pages.Locations;

public partial class EditShiftDialog
{
	[CascadingParameter] public Task<AuthenticationState> AuthenticationState { get; set; } = default!;


	private ClaimsPrincipal? _user;
	private bool _isAdmin;
	private bool _isShiftUser;
	private bool _isShiftBlocked;
	private HashSet<GetShiftTypesResponse> _availableShiftTypes = new();
	private HashSet<GetEmployeeResponse>? _employeesToSelect;


	protected override async Task OnInitializedAsync()
	{
		try
		{
			var auth = await AuthenticationState;
			var container = await ShiftApi.GetContainer(EntityToEdit.ContainerId);
			var dto = await ShiftService.GetAvailableShiftTypesAtTime(EntityToEdit.ContainerId, EntityToEdit.Start);
			//Check User
			_user = auth.User;
			var keycloakId = _user.GetKeycloakId();
			if (keycloakId != default)
			{
				_isShiftUser = keycloakId == EntityToEdit.EmployeeId;
				_isAdmin = _user.IsInRole(ApiRoles.Admin);
			}

			//Find out which ShiftTypes to show
			if (!_isAdmin)
				dto = dto.Where(x => x.OnlyAssignableByAdmin == false);
			_availableShiftTypes = new HashSet<GetShiftTypesResponse>(dto);

			if (EntityToEdit.Type is not null
			    && _availableShiftTypes.All(st => st.Id != EntityToEdit.Type.Id))
				_availableShiftTypes.Add(EntityToEdit.Type);
			if (_availableShiftTypes.Count == 0)
				throw new Exception("Keine freien Schichten für dich verfügbar :(");
			EntityToEdit.Type ??= _availableShiftTypes.First();
			//Set end time
			EntityToEdit.End = EntityToEdit.Start + TimeSpan.FromSeconds(container.Framework.SecondsPerShift);
			//If admin, allow other users to select
			if (_isAdmin)
			{
				var allUser = await ShiftApi.GetAllEmployees();
				_employeesToSelect = new(allUser);
				EntityToEdit.EmployeeId = _employeesToSelect.FirstOrDefault(e => e.Id == EntityToEdit.EmployeeId)?.Id ??
				                          default;
			}

			// Sync blocked state from existing BlockReason
			_isShiftBlocked = !string.IsNullOrEmpty(EntityToEdit.BlockReason);
		}
		catch (AccessTokenNotAvailableException)
		{
			throw;
		}
		catch (Exception ex)
		{
			DialogService.Close(false);
			_ = DialogService.Error(ex);
		}
	}


	protected override async Task Create()
	{
		try
		{
			var res = await ShiftService.AddShiftToContainer(EntityToEdit.ContainerId, new CreateShiftRequest
			{
				EmployeeKeycloakId = _isShiftBlocked ? default : EntityToEdit.EmployeeId,
				ShiftTypeId = EntityToEdit.Type.Id,
				Start = EntityToEdit.Start,
				BlockReason = _isShiftBlocked ? EntityToEdit.BlockReason : null
			});
			EntityToEdit.Id = res;
		}
		catch (ApiException apiException)
		{
			if (apiException.StatusCode == HttpStatusCode.Conflict)
				//TODO Find the shift Guid and catch it and show which location the shift is
				throw new ArgumentException("Du hast schon eine Schicht um diese Zeit!", apiException);
			throw;
		}
	}

	protected override async Task Update()
	{
		if (!(_isAdmin || _isShiftUser))
			throw new UnauthorizedAccessException("Du hast keine Berechtigung, diese Schicht zu bearbeiten.");
		// Prevent unblocking without assigning a user — avoids ghost shifts with placeholder GUIDs
		if (!_isShiftBlocked && EntityToEdit.EmployeeId == Mappers.NotAssignedEmployee.KeycloakId)
		{
			throw new ArgumentException(
				"Bitte wähle einen Mitarbeiter aus, um die Schicht zu entsperren oder lösche diese Schicht um sie frei zu machen.");
		}

		try
		{
			await ShiftApi.UpdateShift(EntityToEdit.Id, new CreateShiftRequest
			{
				EmployeeKeycloakId = EntityToEdit.EmployeeId,
				ShiftTypeId = EntityToEdit.Type.Id,
				Start = EntityToEdit.Start,
				BlockReason = _isShiftBlocked ? EntityToEdit.BlockReason : null
			});
		}
		catch (ApiException apiException)
		{
			if (apiException.StatusCode == HttpStatusCode.Conflict)
				//TODO Find the shift Guid and catch it and show which location the shift is
				throw new ArgumentException("Du hast schon eine Schicht um diese Zeit!", apiException);
			throw;
		}
	}

	private void OnBlockChanged(ChangeEventArgs args)
	{
		_isShiftBlocked = (bool)args.Value;
		// Warn when blocking a shift that has a user assigned
		if (_isShiftBlocked && EntityToEdit.EmployeeId != default)
		{
			// Async operation in event handler - fire and forget for the dialog
			_ = ShowBlockWarningAsync();
		}

		StateHasChanged();
	}

	private async Task ShowBlockWarningAsync()
	{
		var confirmed = await DialogService.Confirm(
			$"{EntityToEdit.EmployeeFullName} wird von dieser Schicht entfernt. Die Schicht wird stattdessen gesperrt.",
			"Schicht sperren");
		if (confirmed is not true)
		{
			_isShiftBlocked = false;
			StateHasChanged();
		}
	}

	public async new Task UpdateAndClose()
	{
		if (_isShiftBlocked && string.IsNullOrWhiteSpace(EntityToEdit.BlockReason))
		{
			await DialogService.Error("Bitte geben Sie eine Begründung für die Sperrung ein.", "Validierung");
			return;
		}

		await base.UpdateAndClose();
	}


	private async Task DeleteClick()
	{
		try
		{
			var ask = await DialogService.Confirm("Bist du sicher, dass du die Schicht löschen willst?");
			if (ask is true)
			{
				await ShiftApi.DeleteShift(EntityToEdit.Id);
				DialogService.Close(true); //return true, as we have changed the entity (deleted it)
			}
		}
		catch (Exception ex)
		{
			await DialogService.Error(ex);
		}
	}
}