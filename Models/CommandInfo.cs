using Botify.Attributes;
using Botify.Interfaces;

namespace Botify.Models;

internal sealed class CommandInfo(
    Func<BotifyContext, Task> @delegate, 
    IReadOnlyList<UseValidatorAttribute> validators) : IValidatable
{
    public Func<BotifyContext, Task> Delegate { get; } = @delegate;
    public IReadOnlyList<UseValidatorAttribute> Validators { get; } = validators;
}
