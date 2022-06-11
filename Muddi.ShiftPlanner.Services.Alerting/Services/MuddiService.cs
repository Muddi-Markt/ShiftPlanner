using Muddi.ShiftPlanner.Shared.Api;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Requests;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses;
using Refit;

namespace Muddi.ShiftPlanner.Services.Alerting.Services;

public class MuddiService
{
	public IMuddiShiftApi ShiftApi { get; }

	private LoginResponse? _token;
	private readonly LoginRequest _loginRequest;

	public MuddiService(IConfiguration configuration)
	{
		_loginRequest = new LoginRequest
		{
			Email = configuration["MuddiShiftApi:User"],
			Password = configuration["MuddiShiftApi:Password"]
		};
		ShiftApi = RestService.For<IMuddiShiftApi>(configuration["MuddiShiftApi:BaseUrl"],
			new RefitSettings { AuthorizationHeaderValueGetter = GetToken });
	}


	private async Task<string> GetToken()
	{
		if (_token is null || DateTime.UtcNow > _token.ExpiresAt)
			_token = await ShiftApi.Login(_loginRequest);
		return _token.AccessToken;
	}
}