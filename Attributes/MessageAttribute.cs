using System.Text.RegularExpressions;
using Telegram.Bot.Types.Enums;

namespace Botify.Attributes;

/// <summary>
/// Помечает метод как обработчик входящих сообщений Telegram.
/// </summary>
/// <remarks>
/// Используется для обработки сообщений,
/// соответствующих указанному регулярному выражению.
///
/// Атрибут применяется только к методам внутри классов,
/// помеченных атрибутом <see cref="MessageHandlerAttribute"/>.
///
/// Поддерживает:
/// <list type="bullet">
/// <item>
/// <description>Фильтрацию по типу сообщения</description>
/// </item>
/// <item>
/// <description>Регулярные выражения</description>
/// </item>
/// <item>
/// <description>Несколько типов сообщений одновременно</description>
/// </item>
/// </list>
///
/// Пример:
/// <code>
/// [Message("^(привет|hello)$")]
/// public async Task HelloMessage(ITelegramBotClient client, Update update, CancellationToken ct)
/// </code>
/// </remarks>
[AttributeUsage(AttributeTargets.Method)]
public class MessageAttribute : Attribute
{
    /// <summary>
    /// Исходный шаблон регулярного выражения.
    /// </summary>
    public string Pattern { get; }

    /// <summary>
    /// Скомпилированное регулярное выражение,
    /// используемое для проверки текста сообщения.
    /// </summary>
    public Regex Regex { get; }

    /// <summary>
    /// Набор типов сообщений,
    /// которые может обрабатывать данный метод.
    /// </summary>
    public HashSet<MessageType> Types { get; }

    /// <summary>
    /// Создаёт атрибут обработчика сообщений
    /// для одного типа сообщения.
    /// </summary>
    /// <param name="pattern">
    /// Регулярное выражение для проверки сообщения.
    /// </param>
    /// <param name="type">
    /// Тип сообщения Telegram.
    /// По умолчанию используется <see cref="MessageType.Text"/>.
    /// </param>
    /// <param name="regexOptions">
    /// Дополнительные параметры регулярного выражения.
    /// По умолчанию используется <see cref="RegexOptions.IgnoreCase"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Выбрасывается, если шаблон равен <see langword="null"/>.
    /// </exception>
    public MessageAttribute(
        string pattern,
        MessageType type = MessageType.Text,
        RegexOptions regexOptions = RegexOptions.IgnoreCase)
    {
        Pattern = pattern;
        Types = [type];
        Regex = new Regex(pattern, regexOptions | RegexOptions.Compiled);
    }

    /// <summary>
    /// Создаёт атрибут обработчика сообщений
    /// для нескольких типов сообщений.
    /// </summary>
    /// <param name="pattern">
    /// Регулярное выражение для проверки сообщения.
    /// </param>
    /// <param name="types">
    /// Массив типов сообщений Telegram.
    /// </param>
    /// <param name="regexOptions">
    /// Дополнительные параметры регулярного выражения.
    /// По умолчанию используется <see cref="RegexOptions.IgnoreCase"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Выбрасывается, если массив типов сообщений равен <see langword="null"/>.
    /// </exception>
    public MessageAttribute(
        string pattern,
        MessageType[] types,
        RegexOptions regexOptions = RegexOptions.IgnoreCase)
    {
        Pattern = pattern;
        Types = [.. types ?? throw new ArgumentNullException(nameof(types))];
        Regex = new Regex(pattern, regexOptions | RegexOptions.Compiled);
    }
}