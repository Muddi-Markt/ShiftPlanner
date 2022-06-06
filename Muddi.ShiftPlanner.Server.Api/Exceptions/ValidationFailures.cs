using System.Security.Cryptography.X509Certificates;
using FluentValidation.Results;
using Muddi.ShiftPlanner.Server.Database.Entities;

namespace Muddi.ShiftPlanner.Server.Api.Exceptions;

public class AlreadyShiftAtGivenTimeFailure : ValidationFailure
{
	public AlreadyShiftAtGivenTimeFailure(ShiftEntity entity)
		: base(entity.Id.ToString(), "User already has shift at given time", new Data(entity))
	{
		ErrorCode = MuddiErrorCodes.AlreadyShiftAtGivenTimeError;
	}

	private record Data(Guid LocationId, string LocationName)
	{
		public Data(ShiftEntity entity) : this(entity.ShiftContainer.Location.Id, entity.ShiftContainer.Location.Name)
		{
		}
	}
}

public static class MuddiErrorCodes
{
	public static string AlreadyShiftAtGivenTimeError = "101";
}