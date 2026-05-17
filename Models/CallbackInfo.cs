using Botify.Attributes;
using Botify.Interfaces;
using System.Reflection;

namespace Botify.Models;

internal sealed class CallbackInfo(
    Func<BotifyContext, Task> @delegate, 
    IReadOnlyList<UseValidatorAttribute> validators) : IValidatable
{
    public Func<BotifyContext, Task> Delegate { get; } = @delegate;
    public IReadOnlyList<UseValidatorAttribute> Validators { get; } = validators;
}