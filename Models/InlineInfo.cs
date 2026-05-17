using System.Reflection;

namespace Botify.Models;

internal class InlineInfo
{
    public Func<BotifyContext, Task> Delegate { get; }

    public InlineInfo(Func<BotifyContext, Task> @delegate)
    {
        Delegate = @delegate;
    }
}