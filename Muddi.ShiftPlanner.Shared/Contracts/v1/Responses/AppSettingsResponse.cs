using Muddi.ShiftPlanner.Shared.Entities;

namespace Muddi.ShiftPlanner.Shared.Contracts.v1.Responses;

public class GetAppSettingsResponse
{
    public ApplicationSettings Settings { get; init; } = new();
}
