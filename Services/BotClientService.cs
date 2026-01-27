using Telegram.Bot;

namespace Botify.Services;

public class BotClientService
{
    public ITelegramBotClient Client { get; }

    public BotClientService(BotifyOptionsBuilder options)
    {
        if (string.IsNullOrWhiteSpace(options.BotToken))
            throw new ArgumentException("BotToken is required", nameof(options.BotToken));

        Client = new TelegramBotClient(options.BotToken);
    }
}
