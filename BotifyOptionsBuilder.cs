using Microsoft.Extensions.Logging;

namespace Botify;

public class BotifyOptionsBuilder
{
    public string BotToken { get; private set; } = string.Empty;
    public char CommandStartChar { get; private set; } = '/';
    public char CallbackSplitChar { get; private set; } = '_';
    public char InlineSplitChar { get; private set; } = ' ';
    
    public ILogger? Logger { get; private set; }
    public LogLevel MinimumLogLevel { get; private set; } = LogLevel.Debug;

    public HttpClientHandler? HttpClientHandler { get; private set; }

    public BotifyOptionsBuilder SetToken(string token)
    {
        BotToken = token;
        return this;
    }

    public BotifyOptionsBuilder SetСommandStartChar(char startChar)
    {
        CommandStartChar = startChar;
        return this;
    }
    public BotifyOptionsBuilder SetHttpClientHandler(HttpClientHandler httpClientHandler)
    {
        HttpClientHandler = httpClientHandler;
        return this;
    }

    public BotifyOptionsBuilder UseLogger(
        ILogger logger,
        LogLevel minimumLevel = LogLevel.Debug)
    {
        Logger = logger;
        MinimumLogLevel = minimumLevel;
        return this;
    }
}
