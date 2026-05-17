using System.Reflection;
using System.Text.RegularExpressions;
using Telegram.Bot.Types.Enums;

namespace Botify.Models;

internal class MessageInfo
{
    public Func<BotifyContext, Task> Delegate { get; }
    public string Pattern { get; set; } = string.Empty;
    public Regex Regex { get; set; } = null!;
    public HashSet<MessageType> Types { get; set; } = [MessageType.Text];

    public MessageInfo(Func<BotifyContext, Task> @delegate)
    {
        Delegate = @delegate;
    }
}
