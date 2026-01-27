using Microsoft.Extensions.Logging;

namespace Botify.Services;

public class LoggerService
{
    private readonly ILogger _logger;
    private readonly BotifyOptionsBuilder _options;

    public LoggerService(BotifyOptionsBuilder options)
    {
        _options = options;
        _logger = options.Logger!;
    }

    public void Log(string message, LogLevel level = LogLevel.Debug)
    {
        if (level < _options.MinimumLogLevel)
            return;

        _logger.Log(level, message);
    }
}
