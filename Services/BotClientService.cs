using Telegram.Bot;

namespace Botify.Services;

internal class BotClientService
{
    public ITelegramBotClient Client { get; }

    public BotClientService(BotifyOptionsBuilder options)
    {
        if (string.IsNullOrWhiteSpace(options.BotToken))
            throw new ArgumentException("BotToken is required", nameof(options.BotToken));


        var httpClient = options.HttpClientHandler != null ? 
            new HttpClient(options.HttpClientHandler) :
            null;

        var tgoptions = new TelegramBotClientOptions(options.BotToken, options.BaseURL);
        Client = new TelegramBotClient(tgoptions, httpClient);
    }
}
