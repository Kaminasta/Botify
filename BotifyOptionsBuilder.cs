using Microsoft.Extensions.Logging;

namespace Botify;

public class BotifyOptionsBuilder
{
    public string BotToken { get; private set; } = string.Empty;
    public char CommandStartChar { get; private set; } = '/';
    public char CallbackSplitChar { get; private set; } = '_';
    
    public ILogger? Logger { get; private set; }
    public LogLevel MinimumLogLevel { get; private set; } = LogLevel.Debug;

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

    public BotifyOptionsBuilder UseLogger(
        ILogger logger,
        LogLevel minimumLevel = LogLevel.Debug)
    {
        Logger = logger;
        MinimumLogLevel = minimumLevel;
        return this;
    }
}
