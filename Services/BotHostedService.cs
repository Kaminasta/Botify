using Botify.Handlers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Botify.Services;

public class BotHostedService : IHostedService
{
    private readonly BotClientService _botClient;
    private readonly CommandHandler _commands;
    private readonly CallbackHandler _callbacks;
    private readonly MessageHandler _messages;
    private readonly LoggerService _logger;

    public BotHostedService(
        BotClientService botClient,
        CommandHandler commands,
        CallbackHandler callbacks,
        MessageHandler messages,
        LoggerService logger)
    {
        _botClient = botClient;
        _commands = commands;
        _callbacks = callbacks;
        _messages = messages;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _botClient.Client.StartReceiving(
            new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync),
            cancellationToken: cancellationToken
        );

        _logger.Log($"Bot started", LogLevel.Debug);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken ct)
    {
        if (update.Message != null)
        {
            bool isHandled = await _commands.HandleAsync(client, update, ct);

            if (!isHandled)
                await _messages.HandleAsync(client, update, ct);
        }
        else if (update.CallbackQuery != null)
            await _callbacks.HandleAsync(client, update, ct);
    }

    private Task HandleErrorAsync(ITelegramBotClient client, Exception ex, CancellationToken ct)
    {
        _logger.Log(ex.ToString(), LogLevel.Error);
        return Task.CompletedTask;
    }
}
