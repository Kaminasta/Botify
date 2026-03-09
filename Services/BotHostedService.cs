using Botify.Handlers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Botify.Services;

public class BotHostedService : IHostedService
{
    private readonly BotClientService _botClient;
    private readonly CommandHandler _commands;
    private readonly CallbackHandler _callbacks;
    private readonly MessageHandler _messages;
    private readonly InlineHandler _inlines;
    private readonly PaymentHandler _payment;
    private readonly LoggerService _logger;

    public BotHostedService(
        BotClientService botClient,
        CommandHandler commands,
        CallbackHandler callbacks,
        MessageHandler messages,
        InlineHandler inlines,
        PaymentHandler payment,
        LoggerService logger)
    {
        _botClient = botClient;
        _commands = commands;
        _callbacks = callbacks;
        _messages = messages;
        _inlines = inlines;
        _payment = payment;
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
            case UpdateType.Message:
                if (update.Message?.SuccessfulPayment != null)
                    await _payment.HandleAsync(client, update, ct);

                bool isHandled = await _commands.HandleAsync(client, update, ct);

                if (!isHandled)
                    await _messages.HandleAsync(client, update, ct);
                break;

            case UpdateType.CallbackQuery:
                await _callbacks.HandleAsync(client, update, ct);
                break;

            case UpdateType.InlineQuery:
                await _inlines.HandleAsync(client, update, ct);
                break;

            case UpdateType.PreCheckoutQuery:
                await _payment.HandleAsync(client, update, ct);
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