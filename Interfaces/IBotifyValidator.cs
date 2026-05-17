namespace Botify.Interfaces;

/// <summary>
/// Представляет валидатор Botify,
/// выполняющий проверку входящего запроса Telegram.
/// </summary>
/// <remarks>
/// Валидаторы используются для выполнения предварительных проверок
/// перед вызовом метода-обработчика.
///
/// Валидатор может быть подключён как к отдельному методу,
/// так и ко всему классу обработчика через атрибуты.
///
///
/// Если метод <see cref="ValidateAsync(BotifyContext)"/>
/// возвращает <see langword="false"/>,
/// выполнение обработчика будет прервано.
///
/// Пример:
/// <code>
/// public class AdminValidator : IBotifyValidator
/// {
///     public Task&lt;bool&gt; ValidateAsync(BotifyContext context)
///     {
///         return Task.FromResult(
///             context.User?.Id == 123456789);
///     }
/// }
/// </code>
/// </remarks>
public interface IBotifyValidator
{
    /// <summary>
    /// Выполняет проверку текущего запроса Telegram.
    /// </summary>
    /// <param name="context">
    /// Контекст текущего запроса,
    /// содержащий информацию о Telegram-обновлении,
    /// клиенте бота, сервисах и других данных.
    /// </param>
    /// <returns>
    /// <see cref="Task"/> с результатом проверки:
    /// <list type="bullet">
    /// <item>
    /// <see langword="true"/> — валидация успешно пройдена,
    /// выполнение обработчика продолжается.
    /// </item>
    /// <item>
    /// <see langword="false"/> — валидация не пройдена,
    /// выполнение обработчика будет остановлено.
    /// </item>
    /// </list>
    /// </returns>
    Task<bool> ValidateAsync(BotifyContext context);
}