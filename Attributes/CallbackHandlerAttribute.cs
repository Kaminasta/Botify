namespace Botify.Attributes;

/// <summary>
/// Помечает класс как контейнер обработчиков callback-запросов Telegram.
/// </summary>
/// <remarks>
/// Классы с данным атрибутом автоматически сканируются Botify
/// для поиска методов, помеченных атрибутом <see cref="CallbackAttribute"/>.
///
/// Используется для группировки обработчиков,
/// работающих с <c>CallbackQuery</c>.
///
/// Пример:
/// <code>
/// [CallbackHandler]
/// public class SettingsCallbacks
/// {
///     [Callback("settings")]
///     public async Task SettingsCallback(...)
/// }
/// </code>
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
public class CallbackHandlerAttribute : Attribute
{
}