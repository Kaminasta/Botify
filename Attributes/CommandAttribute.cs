namespace Botify.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class CommandAttribute : Attribute
{
    public string Name { get; internal set; } = string.Empty;
    public string? Description { get; internal set; }

    public CommandAttribute(string command)
    {
        SetCommand(command);
    }

    public CommandAttribute(string command, string description)
    {
        SetCommand(command);
        Description = description;
    }

    public void SetCommand(string command)
    {
        if (string.IsNullOrWhiteSpace(command))
            throw new ArgumentException("Command cannot be empty", nameof(command));

        command = command.Trim().ToLower();

        if (command.Contains(" "))
            throw new ArgumentException("Command cannot contain spaces", nameof(command));

        Name = command;
    }
}
