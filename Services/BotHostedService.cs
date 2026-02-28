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
    private readonly InlineHandler _inlines;      // ← добавили
    private readonly LoggerService _logger;

    public BotHostedService(
        BotClientService botClient,
        CommandHandler commands,
        CallbackHandler callbacks,
        MessageHandler messages,
        InlineHandler inlines,                     // ← добавили
        LoggerService logger)
    {
        _botClient = botClient;
        _commands = commands;
        _callbacks = callbacks;
        _messages = messages;
        _inlines = inlines;
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
        switch (update.Type)
        {
            case Telegram.Bot.Types.Enums.UpdateType.Message:
                bool isHandled = await _commands.HandleAsync(client, update, ct);

                if (!isHandled)
                    await _messages.HandleAsync(client, update, ct);
                break;

            case Telegram.Bot.Types.Enums.UpdateType.CallbackQuery:
                await _callbacks.HandleAsync(client, update, ct);
                break;

            case Telegram.Bot.Types.Enums.UpdateType.InlineQuery:
                await _inlines.HandleAsync(client, update, ct);
                break;

            default:
                _logger.Log($"Unhandled update type: {update.Type}", LogLevel.Debug);
                break;
        }
    }

    private Task HandleErrorAsync(ITelegramBotClient client, Exception ex, CancellationToken ct)
    {
        _logger.Log(ex.ToString(), LogLevel.Error);
        return Task.CompletedTask;
    }
}