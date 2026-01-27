using System.Reflection;

namespace Botify.Models;

public sealed class CallbackInfo
{
    public object Instance { get; }
    public MethodInfo Method { get; }

    public CallbackInfo(object instance, MethodInfo method)
    {
        Instance = instance;
        Method = method;
    }
}
