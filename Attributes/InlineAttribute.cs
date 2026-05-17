namespace Botify.Attributes;

/// <summary>
/// Помечает метод как обработчик inline-запроса Telegram.
/// </summary>
/// <remarks>
/// Используется для обработки <c>InlineQuery</c>,
/// отправляемых пользователем через inline-режим бота.
///
/// Атрибут применяется только к методам внутри классов,
/// предназначенных для обработки inline-запросов.
///
/// Пример:
/// <code>
/// [Inline("search")]
/// public async Task SearchInline(ITelegramBotClient client, Update update, CancellationToken ct)
/// </code>
/// </remarks>
[AttributeUsage(AttributeTargets.Method)]
public class InlineAttribute : Attribute
{
    /// <summary>
    /// Имя inline-обработчика.
    /// </summary>
    /// <remarks>
    /// Используется для идентификации и маршрутизации
    /// inline-запросов внутри Botify.
    /// Значение автоматически приводится к нижнему регистру
    /// и очищается от пробелов по краям.
    /// </remarks>
    public string Name { get; }

    /// <summary>
    /// Создаёт атрибут обработчика inline-запроса.
    /// </summary>
    /// <param name="name">
    /// Имя inline-обработчика.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Выбрасывается, если имя пустое
    /// или состоит только из пробелов.
    /// </exception>
    public InlineAttribute(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Inline handler name cannot be empty", nameof(name));

        Name = name.Trim().ToLower();
    }
}