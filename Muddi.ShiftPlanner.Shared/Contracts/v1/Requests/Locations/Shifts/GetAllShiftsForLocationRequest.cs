using System.Runtime.Serialization;

namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Requests;

public class GetAllShiftsForLocationRequest
{
	public Guid? Id { get; set; }
	public DateTime Start { get; set; }
	public DateTime End { get; set; }
}