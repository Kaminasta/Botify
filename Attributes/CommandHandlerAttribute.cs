namespace Botify.Attributes;

/// <summary>
/// Помечает класс как контейнер обработчиков Telegram-команд.
/// </summary>
/// <remarks>
/// Классы с данным атрибутом автоматически сканируются Botify
/// для поиска методов, помеченных атрибутом <see cref="CommandAttribute"/>.
///
/// Используется для группировки методов,
/// обрабатывающих команды Telegram.
///
/// Пример:
/// <code>
/// [CommandHandler]
/// public class UserCommands
/// {
///     [Command("start", "Начать работу с ботом")]
///     public async Task StartCommand(...)
/// }
/// </code>
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
public class CommandHandlerAttribute : Attribute
{
}