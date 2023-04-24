using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses.Seasons;

namespace Muddi.ShiftPlanner.Shared.Entities;

public class Season
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public DateTime StartDate { get; set; }
	public DateTime EndDate { get; set; }
	public bool IsSelected { get; set; }

	public static Season FromResponse(GetSeasonResponse last)
    {
        return new Season
        {
            Id = last.Id,
            Name = last.Name,
            StartDate = last.StartDate,
            EndDate = last.EndDate,
            IsSelected = last.IsSelectedAsCurrent
        };
    }
}