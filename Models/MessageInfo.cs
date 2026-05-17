using System.Reflection;
using System.Text.RegularExpressions;
using Telegram.Bot.Types.Enums;

namespace Botify.Models;

internal class MessageInfo
{
    public object Instance { get; set; } = null!;
    public MethodInfo Method { get; set; } = null!;
    public string Pattern { get; set; } = string.Empty;
    public Regex Regex { get; set; } = null!;
    public HashSet<MessageType> Types { get; set; } = [MessageType.Text];
}
