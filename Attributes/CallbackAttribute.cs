namespace Botify.Attributes;

/// <summary>
/// Атрибут для регистрации метода как обработчика callback-запроса Telegram.
/// </summary>
/// <remarks>
/// Используется для обработки данных,
/// передаваемых через <c>CallbackQuery.Data</c>.
///
/// Атрибут применяется только к методам внутри классов,
/// помеченных атрибутом <see cref="CallbackHandlerAttribute"/>.
///
/// Пример:
/// <code>
/// [Callback("settings")]
/// public async Task SettingsCallback(BotifyContext context)
/// </code>
/// </remarks>
[AttributeUsage(AttributeTargets.Method)]
public class CallbackAttribute : Attribute
{
    /// <summary>
    /// Имя callback-запроса.
    /// </summary>
    /// <remarks>
    /// Значение используется для сопоставления
    /// входящего callback-запроса с методом-обработчиком.
    /// </remarks>
    public string Name { get; }

    /// <summary>
    /// Создаёт атрибут обработчика callback-запроса.
    /// </summary>
    /// <param name="callback">
    /// Имя callback-запроса.
    /// Значение не может быть пустым и содержать пробелы.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Выбрасывается, если callback пустой
    /// или содержит пробелы.
    /// </exception>
    public CallbackAttribute(string callback)
    {
        if (string.IsNullOrWhiteSpace(callback))
            throw new ArgumentException("Callback cannot be empty", nameof(callback));

        callback = callback.Trim().ToLower();

        if (callback.Contains(" "))
            throw new ArgumentException("Callback cannot contain spaces", nameof(callback));

        Name = callback;
    }
}