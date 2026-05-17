namespace Botify.Attributes;

/// <summary>
/// Помечает класс как контейнер обработчиков inline-запросов Telegram.
/// </summary>
/// <remarks>
/// Классы с данным атрибутом автоматически сканируются Botify
/// для поиска методов, помеченных атрибутом <see cref="InlineAttribute"/>.
///
/// Используется для группировки обработчиков,
/// работающих с <c>InlineQuery</c>.
///
/// Пример:
/// <code>
/// [InlineHandler]
/// public class SearchInlineHandler
/// {
///     [Inline("search")]
///     public async Task Search(...)
/// }
/// </code>
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
public class InlineHandlerAttribute : Attribute
{
}