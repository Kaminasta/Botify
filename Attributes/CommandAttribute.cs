namespace Botify.Attributes;

/// <summary>
/// Атрибут для регистрации метода как обработчика Telegram-команды.
/// </summary>
/// <remarks>
/// Используется только для методов внутри классов,
/// помеченных атрибутом <see cref="CommandHandlerAttribute"/>.
///
/// Пример:
/// <code>
/// [Command("start", "Начать работу с ботом")]
/// public async Task StartCommand(BotifyContext context)
/// </code>
/// </remarks>
[AttributeUsage(AttributeTargets.Method)]
public class CommandAttribute : Attribute
{
    /// <summary>
    /// Имя команды без символа '/'.
    /// </summary>
    /// <example>
    /// Для команды "/start" значение будет "start".
    /// </example>
    public string Name { get; }

    /// <summary>
    /// Описание команды, отображаемое в интерфейсе Telegram.
    /// </summary>
    /// <remarks>
    /// Может быть <see langword="null"/>, если описание не требуется.
    /// </remarks>
    public string? Description { get; }

    /// <summary>
    /// Создаёт атрибут обработчика команды.
    /// </summary>
    /// <param name="command">
    /// Имя команды без символа '/'.
    /// Команда не может быть пустой и содержать пробелы.
    /// </param>
    /// <param name="description">
    /// Необязательное описание команды,
    /// отображаемое в меню Telegram.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Выбрасывается, если имя команды пустое
    /// или содержит пробелы.
    /// </exception>
    public CommandAttribute(string command, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(command))
            throw new ArgumentException("Command cannot be empty", nameof(command));

        command = command.Trim().ToLower();

        if (command.Contains(" "))
            throw new ArgumentException("Command cannot contain spaces", nameof(command));

        Name = command;

        Description = description;
    }
}