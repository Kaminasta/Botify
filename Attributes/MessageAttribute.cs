using System.Text.RegularExpressions;
using Telegram.Bot.Types.Enums;

namespace Botify.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class MessageAttribute : Attribute
{
    public string Pattern { get; }
    public Regex Regex { get; }
    public HashSet<MessageType> Types { get; }

    public MessageAttribute(
        string pattern, 
        MessageType type = MessageType.Text, 
        RegexOptions regexOptions = RegexOptions.IgnoreCase)
    {
        Pattern = pattern;
        Types = [type];
        Regex = new Regex(pattern, regexOptions | RegexOptions.Compiled);
    }

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
