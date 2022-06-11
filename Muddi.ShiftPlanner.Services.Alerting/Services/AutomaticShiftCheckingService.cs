﻿using System.Globalization;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Muddi.ShiftPlanner.Shared.Api;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Requests;
using Muddi.ShiftPlanner.Shared.Contracts.v1.Responses;
using Refit;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Muddi.ShiftPlanner.Services.Alerting.Services;

public class AutomaticShiftCheckingService : IHostedService
{
	private readonly ITelegramBotClient _telegramClient;
	private readonly MuddiService _muddi;
	private readonly Timer _timer;
	private readonly LoginRequest _loginRequest;
	private readonly ILogger<AutomaticShiftCheckingService> _logger;
	private readonly LocationsService _locations;
	private readonly string _muddiClientBaseUri;
	private readonly ChatId _muddiGroupId;
	private readonly DateTime _startDate;

	public AutomaticShiftCheckingService(
		IConfiguration configuration,
		MuddiService muddi,
		ITelegramBotClient telegramClient,
		LocationsService locations,
		ILogger<AutomaticShiftCheckingService> logger)
	{
		_muddiClientBaseUri = configuration["MuddiShiftClient:BaseUrl"].TrimEnd('/');

		_muddi = muddi;
		_telegramClient = telegramClient;
		_logger = logger;
		_locations = locations;
		_muddiGroupId = new ChatId(configuration.GetSection("Telegram").GetValue<long>("GroupId"));
		_startDate = DateTime.ParseExact(configuration["Telegram:StartDate"], "dd.MM.yyyy", CultureInfo.InvariantCulture);
		
		var sendTime = TimeSpan.Parse(configuration["Telegram:SendTime"]);
		var dueTime = CalculateDueTime(sendTime);
		Execute(null); //execute directly
		_timer = new Timer(Execute, null, dueTime, TimeSpan.FromHours(24));
	}

	public static TimeSpan CalculateDueTime(TimeSpan timeOfDayToStart)
	{
		var due = timeOfDayToStart > DateTime.UtcNow.TimeOfDay
			? timeOfDayToStart - DateTime.UtcNow.TimeOfDay
			: timeOfDayToStart.Add(TimeSpan.FromDays(1)) - DateTime.UtcNow.TimeOfDay;

		return due;
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
			var allAvailableShifts = await _muddi.ShiftApi.GetAvailableShiftTypes(new GetAllShiftsRequest());
			// var tomorrow = new DateTime(2022, 06, 17).Date;
			var tomorrow = DateTime.UtcNow.AddDays(1).Date;
			if (tomorrow < _startDate)
				tomorrow = _startDate;
			var allShiftsTomorrow = allAvailableShifts.Where(s => s.Start.Date == tomorrow && s.AvailableCount > 0).ToList();
			if (!allShiftsTomorrow.Any())
			{
				_logger.LogInformation("No shifts for {Tomorrow}", tomorrow);
				await _telegramClient.SendTextMessageAsync(_muddiGroupId, "Alle Schichten besetzt :)", ParseMode.MarkdownV2);
			}
			else
			{
				_logger.LogInformation("Found {N} free shifts for tomorrow", allShiftsTomorrow.Count);
				var text = await CreateTextForBot(allShiftsTomorrow,tomorrow);
				await _telegramClient.SendTextMessageAsync(new ChatId(-794685427), text, ParseMode.MarkdownV2);
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to execute timer method");
		}
	}

	private async Task<string> CreateTextForBot(List<GetShiftTypesCountResponse> shifts, DateTime tomorrow)
	{
		StringBuilder sb = new();
		sb.Append($"⚠️Für den nächsten KiWo Tag \\({tomorrow.Day}\\.{tomorrow.Month}\\.\\) gibt es noch {shifts.Count} freie Schichten\\.⚠️\n");
		foreach (var locations in shifts.GroupBy(s => s.LocationId))
		{
			var name = await _locations.GetName(locations.Key).ConfigureAwait(false);
			sb.Append($"\t[{name}]({_muddiClientBaseUri}/locations/{locations.Key}):\t{locations.Count()} freie Schichten\n");
		}

		return sb.ToString();
	}


	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}
}