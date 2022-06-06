using System.Diagnostics;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Muddi.ShiftPlanner.Shared;
using Muddi.ShiftPlanner.Shared.Contracts.v1;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Requests;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses;
using Radzen;
using Refit;

namespace Muddi.ShiftPlanner.Client.Pages.Locations;

public partial class EditShiftDialog
{
	[CascadingParameter] public Task<AuthenticationState> AuthenticationState { get; set; } = default!;


	private ClaimsPrincipal? _user;
	private bool _isAdmin;
	private bool _isShiftUser;
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
				_isShiftUser = keycloakId == EntityToEdit.Employee?.Id;
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
				EntityToEdit.Employee = _employeesToSelect.FirstOrDefault(e => e.Id == EntityToEdit.Employee.Id);
			}
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
				EmployeeKeycloakId = EntityToEdit.Employee.Id,
				ShiftTypeId = EntityToEdit.Type.Id,
				Start = EntityToEdit.Start
			});
			EntityToEdit.Id = res;
		}
		catch (ApiException apiException)
		{
			if (apiException.StatusCode != HttpStatusCode.Conflict) throw;
			//TODO Find the shift Guid and catch it and show which location the shift is
			Console.WriteLine(apiException.Content);
			await DialogService.Error("Du hast schon ne Schicht um die Zeit!");
		}
	}

	protected override async Task Update()
	{
		if (!(_isAdmin || _isShiftUser))
			return;
		try
		{
			await ShiftApi.UpdateShift(EntityToEdit.Id, new CreateShiftRequest
			{
				EmployeeKeycloakId = EntityToEdit.Employee.Id,
				ShiftTypeId = EntityToEdit.Type.Id,
				Start = EntityToEdit.Start
			});
		}
		catch (ApiException apiException)
		{
			if (apiException.StatusCode != HttpStatusCode.Conflict) throw;
			//TODO Find the shift Guid and catch it and show which location the shift is
			await DialogService.Error("Du hast schon ne Schicht um die Zeit!");
		}
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