using Telegram.Bot;

namespace Muddi.ShiftPlanner.Services.Alerting.Extensions;

public static class ServiceCollectionExtensions
{
	public static void AddTelegramBot(this IServiceCollection services, IConfiguration configuration)
	{
		var config = configuration.GetSection("Telegram").Get<TelegramBotConfig>();
		services.AddHttpClient("tgwebhook")
			.AddTypedClient<ITelegramBotClient>(httpClient => new TelegramBotClient(config.ApiToken, httpClient));
		
	}
}

public class TelegramBotConfig
{
	public string ApiToken { get; init; } = default!;
}