using Botify.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Botify;

/// <summary>
/// Контекст обработки Telegram-обновления в Botify.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="BotifyContext"/> предоставляет единый объект,
/// содержащий все данные и сервисы,
/// необходимые для обработки входящего обновления Telegram.
/// </para>
///
/// <para>
/// Контекст автоматически создаётся Botify
/// для каждого входящего <see cref="Update"/>
/// и передаётся в обработчики, валидаторы,
/// middleware и фильтры.
/// </para>
///
/// </remarks>
public sealed class BotifyContext
{
    /// <summary>
    /// Telegram Bot API клиент.
    /// </summary>
    public required ITelegramBotClient Client { get; init; }

    /// <summary>
    /// Входящее Telegram-обновление.
    /// </summary>
    public required Update Update { get; init; }

    /// <summary>
    /// Провайдер сервисов приложения.
    /// </summary>
    /// <remarks>
    /// Позволяет получать зарегистрированные сервисы
    /// через Dependency Injection.
    /// </remarks>
    public required IServiceProvider Services { get; init; }

    /// <summary>
    /// Токен отмены обработки запроса.
    /// </summary>
    /// <remarks>
    /// Используется для корректной отмены
    /// асинхронных операций.
    /// </remarks>
    public required CancellationToken CancellationToken { get; init; }

    /// <summary>
    /// Конфигурация Botify.
    /// </summary>
    public required BotifyOptionsBuilder Options { get; init; }

    /// <summary>
    /// Сервис логирования Botify.
    /// </summary>
    public required LoggerService Logger { get; init; }

    /// <summary>
    /// Telegram-пользователь, инициировавший обновление.
    /// </summary>
    /// <remarks>
    /// Автоматически извлекается из:
    /// <list type="bullet">
    /// <item><description><see cref="Update.Message"/></description></item>
    /// <item><description><see cref="Update.CallbackQuery"/></description></item>
    /// <item><description><see cref="Update.InlineQuery"/></description></item>
    /// </list>
    /// </remarks>
    public User? User =>
        Update.Message?.From ??
        Update.CallbackQuery?.From ??
        Update.InlineQuery?.From;

    /// <summary>
    /// Telegram-чат текущего обновления.
    /// </summary>
    public Chat? Chat =>
        Update.Message?.Chat ??
        Update.CallbackQuery?.Message?.Chat;

    /// <summary>
    /// Telegram-сообщение текущего обновления.
    /// </summary>
    public Message? Message =>
        Update.Message ??
        Update.CallbackQuery?.Message;

    /// <summary>
    /// Текст сообщения текущего обновления.
    /// </summary>
    public string? Text =>
        Message?.Text ?? 
        Message?.Caption;

    /// <summary>
    /// Callback-запрос текущего обновления.
    /// </summary>
    public CallbackQuery? CallbackQuery =>
        Update.CallbackQuery;

    /// <summary>
    /// Inline-запрос текущего обновления.
    /// </summary>
    public InlineQuery? InlineQuery =>
        Update.InlineQuery;
}