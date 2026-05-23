using System.Text.Json;
using System.Text.Json.Serialization;
using Muddi.ShiftPlanner.Shared.Api;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Requests;
using Muddi.ShiftPlanner.Shared.Entities;
using Microsoft.JSInterop;

namespace Muddi.ShiftPlanner.Client.Services;

/// <summary>
/// Provides app-wide settings (Title, Subtitle, Contact, StartTime, EndTime, Greeting, MemberName).
/// Loads the values from the API once and caches them. If the API is unreachable
/// (e.g. called before authentication) it falls back to <see cref="Defaults"/> without
/// caching the failure, so a later call retries.
/// </summary>
public class AppSettingsService
{
	private const string LocalStorageKey = "appSettings";

	/// <summary>
	/// Values used until the real settings load, or whenever the API is unreachable.
	/// </summary>
	private static readonly ApplicationSettings Defaults = new();

	private static readonly JsonSerializerOptions JsonSerializerOptions = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase
	};

	private readonly IMuddiShiftApi _api;
	private readonly IJSRuntime _jsRuntime;
	private readonly ILogger<AppSettingsService> _logger;
	private Lazy<Task<ApplicationSettings>> _lazySettings;


	/// <summary>
	/// Raised when the effective settings change — on the first successful load and on every update.
	/// </summary>
	public event EventHandler<ApplicationSettings>? SettingsChanged;

	/// <summary>
	/// The most recently known settings. Equals <see cref="Defaults"/> until the first successful load.
	/// </summary>
	public ApplicationSettings Current { get; private set; } = Defaults;

	public AppSettingsService(IMuddiShiftApi api, IJSRuntime jsRuntime, ILogger<AppSettingsService> logger)
	{
		_api = api;
		_jsRuntime = jsRuntime;
		_logger = logger;
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

	public async Task UpdateAsync(ApplicationSettings settings)
	{
		await _api.UpdateAppSettings(new UpdateAppSettingsRequest
		{
			Title = settings.Title,
			Subtitle = settings.Subtitle,
			Contact = settings.Contact,
			StartTime = settings.StartTime,
			EndTime = settings.EndTime,
			Greeting = settings.Greeting,
			MemberName = settings.MemberName
		});

		// Re-fetch so callers and the SettingsChanged event see the persisted values.
		_lazySettings = new Lazy<Task<ApplicationSettings>>(LoadAsync);
		await _lazySettings.Value;
	}

	private async Task<ApplicationSettings> LoadAsync()
	{
		var response = await _api.GetAppSettings();
		SetCurrent(response.Settings);
		await SaveToLocalStorageAsync(response.Settings);
		return response.Settings;
	}

	private async Task SaveToLocalStorageAsync(ApplicationSettings settings)
	{
		try
		{
			var json = JsonSerializer.Serialize(settings, JsonSerializerOptions);
			await _jsRuntime.InvokeVoidAsync("localStorage.setItem", LocalStorageKey, json);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to save settings to localStorage");
		}
	}

	/// <summary>
	/// Loads settings from localStorage, returning null if nothing is cached.
	/// </summary>
	public async Task<ApplicationSettings?> GetCachedSettingsAsync()
	{
		try
		{
			var json = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", LocalStorageKey);
			if (string.IsNullOrEmpty(json)) return null;
			return JsonSerializer.Deserialize<ApplicationSettings>(json, JsonSerializerOptions);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to load settings from localStorage");
			return null;
		}
	}

	private void SetCurrent(ApplicationSettings settings)
	{
		if (Current == settings)
			return;
		Current = settings;
		SettingsChanged?.Invoke(this, settings);
	}
}