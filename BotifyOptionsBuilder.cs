using Microsoft.Extensions.Logging;

namespace Botify;

/// <summary>
/// Построитель конфигурации Botify.
/// </summary>
/// <remarks>
/// Используется для настройки параметров Telegram-бота,
/// логирования и HTTP-клиента.
///
/// Экземпляр конфигурации создаётся автоматически
/// при вызове <c>AddBotify(...)</c>.
/// </remarks>
public class BotifyOptionsBuilder
{
    /// <summary>
    /// Токен Telegram-бота.
    /// </summary>
    public string BotToken { get; private set; } = string.Empty;

    /// <summary>
    /// Пользовательский базовый URL Telegram API.
    /// </summary>
    /// <remarks>
    /// Может использоваться для локальных Bot API серверов
    /// или прокси.
    /// </remarks>
    public string? BaseURL { get; private set; } = default;

    /// <summary>
    /// Символ начала команды.
    /// </summary>
    /// <remarks>
    /// По умолчанию используется символ '/'.
    /// </remarks>
    public char CommandStartChar { get; private set; } = '/';

    /// <summary>
    /// Символ-разделитель callback-данных.
    /// </summary>
    /// <remarks>
    /// Используется при разборе <c>CallbackQuery.Data</c>.
    /// </remarks>
    public char CallbackSplitChar { get; private set; } = '_';

    /// <summary>
    /// Символ-разделитель inline-запросов.
    /// </summary>
    /// <remarks>
    /// Используется при разборе текста inline-запроса.
    /// </remarks>
    public char InlineSplitChar { get; private set; } = ' ';

    /// <summary>
    /// Экземпляр логгера Botify.
    /// </summary>
    public ILogger? Logger { get; private set; }

    /// <summary>
    /// Минимальный уровень логирования.
    /// </summary>
    public LogLevel MinimumLogLevel { get; private set; } = LogLevel.Debug;

    /// <summary>
    /// Пользовательский HTTP handler.
    /// </summary>
    /// <remarks>
    /// Может использоваться для настройки proxy,
    /// SSL или других HTTP-параметров.
    /// </remarks>
    public HttpClientHandler? HttpClientHandler { get; private set; }

    public Func<BotifyContext, Task>? UnknownCommandHandler { get; set; }

    public Func<BotifyContext, Task>? UnknownCallbackHandler { get; set; }

    public Func<BotifyContext, Task>? UnknownInlineHandler { get; set; }

    /// <summary>
    /// Устанавливает токен Telegram-бота.
    /// </summary>
    /// <param name="token">
    /// Токен бота Telegram.
    /// </param>
    /// <returns>
    /// Текущий экземпляр <see cref="BotifyOptionsBuilder"/>.
    /// </returns>
    public BotifyOptionsBuilder SetToken(string token)
    {
        BotToken = token;
        return this;
    }

    /// <summary>
    /// Устанавливает пользовательский базовый URL Telegram API.
    /// </summary>
    /// <param name="baseUrl">
    /// URL Telegram Bot API.
    /// </param>
    /// <returns>
    /// Текущий экземпляр <see cref="BotifyOptionsBuilder"/>.
    /// </returns>
    public BotifyOptionsBuilder SetBaseURL(string baseUrl)
    {
        BaseURL = baseUrl;
        return this;
    }

    /// <summary>
    /// Устанавливает символ начала Telegram-команд.
    /// </summary>
    /// <param name="startChar">
    /// Символ начала команды.
    /// </param>
    /// <returns>
    /// Текущий экземпляр <see cref="BotifyOptionsBuilder"/>.
    /// </returns>
    public BotifyOptionsBuilder SetСommandStartChar(char startChar)
    {
        CommandStartChar = startChar;
        return this;
    }

    /// <summary>
    /// Устанавливает пользовательский HTTP handler.
    /// </summary>
    /// <param name="httpClientHandler">
    /// Экземпляр HTTP handler.
    /// </param>
    /// <returns>
    /// Текущий экземпляр <see cref="BotifyOptionsBuilder"/>.
    /// </returns>
    public BotifyOptionsBuilder SetHttpClientHandler(HttpClientHandler httpClientHandler)
    {
        HttpClientHandler = httpClientHandler;
        return this;
    }

    /// <summary>
    /// Подключает систему логирования.
    /// </summary>
    /// <param name="logger">
    /// Экземпляр логгера.
    /// </param>
    /// <param name="minimumLevel">
    /// Минимальный уровень логирования.
    /// </param>
    /// <returns>
    /// Текущий экземпляр <see cref="BotifyOptionsBuilder"/>.
    /// </returns>
    public BotifyOptionsBuilder UseLogger(
        ILogger logger,
        LogLevel minimumLevel = LogLevel.Debug)
    {
        Logger = logger;
        MinimumLogLevel = minimumLevel;

        return this;
    }
}