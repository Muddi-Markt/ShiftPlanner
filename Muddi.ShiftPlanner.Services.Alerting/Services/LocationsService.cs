using Microsoft.Extensions.Caching.Memory;

namespace Muddi.ShiftPlanner.Services.Alerting.Services;

public class LocationsService
{
	private readonly IMemoryCache _cache;
	private readonly MuddiService _muddi;

	public LocationsService(IMemoryCache cache, MuddiService muddi)
	{
		_cache = cache;
		_muddi = muddi;
	}

	public async ValueTask<string> GetName(Guid locationsKey)
	{
		if (!_cache.TryGetValue(locationsKey, out string name))
		{
			var res = await _muddi.ShiftApi.GetLocationById(locationsKey);
			name = res.Name;
			_cache.Set(locationsKey, name, TimeSpan.FromDays(30));
		}

		return name;
	}
}
