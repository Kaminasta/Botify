using Botify.Attributes;
using Botify.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;
using Telegram.Bot.Types.Enums;

namespace Botify.Models;

internal sealed class MessageInfo(
        Func<BotifyContext, Task> @delegate,
        IReadOnlyList<UseValidatorAttribute> validators) : IValidatable
{
    public Func<BotifyContext, Task> Delegate { get; } = @delegate;
    public IReadOnlyList<UseValidatorAttribute> Validators { get; } = validators;

    public string Pattern { get; set; } = string.Empty;
    public Regex Regex { get; set; } = null!;
    public HashSet<MessageType> Types { get; set; } = [MessageType.Text];
}
