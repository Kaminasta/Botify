using System.Reflection;

namespace Botify.Models;

internal sealed class CallbackInfo
{
    public Func<BotifyContext, Task> Delegate { get; }

    public CallbackInfo(Func<BotifyContext, Task> @delegate)
    {
        Delegate = @delegate;
    }
}