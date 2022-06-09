using Muddi.ShiftPlanner.Shared.Api;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Requests;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses;
using Refit;

namespace Muddi.ShiftPlanner.Services.Alerting.Services;

public class AutomaticShiftCheckingService : IHostedService
{
	private readonly IMuddiShiftApi _shiftApi;
	private readonly Timer _timer;
	private LoginResponse? _token;
	private readonly LoginRequest _loginRequest;
	private ILogger<AutomaticShiftCheckingService> _logger;

	public AutomaticShiftCheckingService(IConfiguration configuration,
		ILogger<AutomaticShiftCheckingService> logger)
	{
		_loginRequest = new LoginRequest
		{
			Email = configuration["MuddiShiftApi:User"],
			Password = configuration["MuddiShiftApi:Password"]
		};
		_shiftApi = RestService.For<IMuddiShiftApi>(configuration["MuddiShiftApi:BaseUrl"],
			new RefitSettings { AuthorizationHeaderValueGetter = GetToken });

		_logger = logger;
		_timer = new Timer(Execute, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
	}

	private async Task<string> GetToken()
	{
		if (_token is null || DateTime.UtcNow > _token.ExpiresAt)
			_token = await _shiftApi.Login(_loginRequest);
		return _token.AccessToken;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		cancellationToken.Register(() => _timer.Dispose());
		return Task.CompletedTask;
	}


	public async void Execute(object? o)
	{
		try
		{
			var allAvailableShifts = await _shiftApi.GetAvailableShiftTypes(new GetAllShiftsRequest());
			var tomorrow = new DateTime(2022, 06, 17).Date;
			// var tomorrow = DateTime.UtcNow.AddDays(1).Date;
			var allShiftsTomorrow = allAvailableShifts.Where(s => s.Start.Date == tomorrow && s.AvailableCount > 0).ToList();
			if (!allShiftsTomorrow.Any())
			{
				_logger.LogInformation("No shifts for {Tomorrow}", tomorrow);
			}
			else
			{
				_logger.LogInformation("Found {N} free shifts for tomorrow", allShiftsTomorrow.Count);
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to execute timer method");
		}
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}
}