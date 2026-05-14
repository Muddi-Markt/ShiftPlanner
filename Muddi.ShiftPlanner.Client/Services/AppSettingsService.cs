using Muddi.ShiftPlanner.Shared.Api;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Requests;
using Muddi.ShiftPlanner.Shared.Entities;

namespace Muddi.ShiftPlanner.Client.Services;

/// <summary>
/// Provides app-wide settings (StartTime, EndTime).
/// Loads the values from the API once and caches them. If the API is unreachable
/// (e.g. called before authentication) it falls back to <see cref="Defaults"/> without
/// caching the failure, so a later call retries.
/// </summary>
public class AppSettingsService
{
	/// <summary>
	/// Values used until the real settings load, or whenever the API is unreachable.
	/// </summary>
	public static ApplicationSettings Defaults { get; } = new()
	{
		StartTime = new TimeSpan(8, 0, 0),
		EndTime = new TimeSpan(26, 0, 0)
	};

	private readonly IMuddiShiftApi _api;
	private Lazy<Task<ApplicationSettings>> _lazySettings;
	private ApplicationSettings _current = Defaults;

	/// <summary>
	/// Raised when the effective settings change — on the first successful load and on every update.
	/// </summary>
	public event EventHandler<ApplicationSettings>? SettingsChanged;

	/// <summary>
	/// The most recently known settings. Equals <see cref="Defaults"/> until the first successful load.
	/// </summary>
	public ApplicationSettings Current => _current;

	public AppSettingsService(IMuddiShiftApi api)
	{
		_api = api;
		_lazySettings = new Lazy<Task<ApplicationSettings>>(LoadAsync);
	}

	/// <summary>
	/// Returns the current settings, loading them from the API once. Falls back to
	/// <see cref="Defaults"/> if the API is unreachable; the failed load is not cached,
	/// so a later call retries instead of replaying the failure.
	/// </summary>
	public async ValueTask<ApplicationSettings> GetSettingsAsync()
	{
		var lazy = _lazySettings;
		try
		{
			return await lazy.Value;
		}
		catch
		{
			// Drop the faulted task so the next call retries instead of replaying the failure.
			if (_lazySettings == lazy)
				_lazySettings = new Lazy<Task<ApplicationSettings>>(LoadAsync);
			return Defaults;
		}
	}

	public async Task UpdateAsync(TimeSpan startTime, TimeSpan endTime)
	{
		await _api.UpdateAppSettings(new UpdateAppSettingsRequest
		{
			StartTime = startTime,
			EndTime = endTime
		});

		// Re-fetch so callers and the SettingsChanged event see the persisted values.
		_lazySettings = new Lazy<Task<ApplicationSettings>>(LoadAsync);
		await _lazySettings.Value;
	}

	private async Task<ApplicationSettings> LoadAsync()
	{
		var response = await _api.GetAppSettings();
		SetCurrent(response.Settings);
		return response.Settings;
	}

	private void SetCurrent(ApplicationSettings settings)
	{
		if (_current == settings)
			return;
		_current = settings;
		SettingsChanged?.Invoke(this, settings);
	}
}
