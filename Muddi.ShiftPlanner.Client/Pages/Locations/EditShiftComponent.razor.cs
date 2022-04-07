using Microsoft.AspNetCore.Components;
using Muddi.ShiftPlanner.Client.Entities;
using Muddi.ShiftPlanner.Shared.Entities;

namespace Muddi.ShiftPlanner.Client.Pages.Locations;

public partial class EditShiftComponent
{
	[Parameter] public TemplateFormShiftParameter ShiftParameter { get; set; }

	private bool isAllowedToEdit;

	protected override void OnParametersSet()
	{
		if (ShiftParameter is null) throw new ArgumentNullException(nameof(ShiftParameter));
		_availableRoles.Clear();
		_availableRoles.AddRange(ShiftParameter.Container.GetAvailableRolesAtGivenTime(ShiftParameter.StartTime));
		isAllowedToEdit = ShiftParameter.User.UserRole >= UserRoles.Manager
		                  || ShiftParameter.ShiftToEdit is null
		                  || (ShiftParameter.ShiftToEdit is { } shift && shift.User == ShiftParameter.User);

		ShiftParameter.Role ??= ShiftParameter.ShiftToEdit?.Role ?? _availableRoles.FirstOrDefault();
		
		if (isAllowedToEdit && ShiftParameter.Role is not null && !_availableRoles.Contains(ShiftParameter.Role))
			_availableRoles.Add(ShiftParameter.Role);
	}

	void OnSubmit(TemplateFormShiftParameter model)
	{
		DialogService.Close(model);
	}


	private List<ShiftRole> _availableRoles = new();
}