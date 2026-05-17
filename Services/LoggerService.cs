using Microsoft.Extensions.Logging;

namespace Botify.Services;

/// <summary>
/// Сервис логирования Botify.
/// </summary>
/// <remarks>
/// Предоставляет удобный способ записи логов
/// через систему логирования <see cref="Microsoft.Extensions.Logging"/>.
///
/// Учитывает минимальный уровень логирования,
/// заданный в <see cref="BotifyOptionsBuilder"/>.
///
/// Может использоваться как внутри Botify,
/// так и в пользовательском коде.
///
/// Пример:
/// <code>
/// public class MyService
/// {
///     private readonly LoggerService _logger;
///
///     public MyService(LoggerService logger)
///     {
///         _logger = logger;
///     }
///
///     public void Test()
///     {
///         _logger.Log("Сообщение");
///     }
/// }
/// </code>
/// </remarks>
public class LoggerService
{
    private readonly ILogger _logger;
    private readonly BotifyOptionsBuilder _options;

    /// <summary>
    /// Инициализирует сервис логирования.
    /// </summary>
    /// <param name="options">
    /// Конфигурация Botify.
    /// </param>
    public LoggerService(BotifyOptionsBuilder options)
    {
        _options = options;
        _logger = options.Logger!;
    }

    /// <summary>
    /// Записывает сообщение в лог.
    /// </summary>
    /// <param name="message">
    /// Текст сообщения.
    /// </param>
    /// <param name="level">
    /// Уровень логирования.
    /// По умолчанию используется <see cref="LogLevel.Debug"/>.
    /// </param>
    public void Log(string message, LogLevel level = LogLevel.Debug)
    {
        if (level < _options.MinimumLogLevel)
            return;

        _logger.Log(level, message);
    }
}