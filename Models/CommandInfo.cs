using System.Reflection;

namespace Botify.Models;

internal class CommandInfo
{
    public Func<BotifyContext, Task> Delegate { get; }

    public CommandInfo(Func<BotifyContext, Task> @delegate)
    {
        Delegate = @delegate;
    }
}
