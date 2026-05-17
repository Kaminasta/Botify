namespace Botify.Attributes;

/// <summary>
/// Помечает класс как контейнер обработчиков входящих сообщений Telegram.
/// </summary>
/// <remarks>
/// Классы с данным атрибутом автоматически сканируются Botify
/// для поиска методов, помеченных атрибутом <see cref="MessageAttribute"/>.
///
/// Используется для группировки методов,
/// обрабатывающих входящие сообщения пользователя.
///
/// Поддерживаются:
/// <list type="bullet">
/// <item>
/// <description>Текстовые сообщения</description>
/// </item>
/// <item>
/// <description>Медиа-сообщения</description>
/// </item>
/// <item>
/// <description>Фильтрация по типу сообщения</description>
/// </item>
/// <item>
/// <description>Маршрутизация через регулярные выражения</description>
/// </item>
/// </list>
///
/// Пример:
/// <code>
/// [MessageHandler]
/// public class UserMessages
/// {
///     [Message("^привет$")]
///     public async Task Hello(...)
/// }
/// </code>
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
public class MessageHandlerAttribute : Attribute
{
}