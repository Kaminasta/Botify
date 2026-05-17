using System.Reflection;

namespace Botify.Models;

internal class CallbackInfo
{
    public object Instance { get; }
    public MethodInfo Method { get; }

    public CallbackInfo(object instance, MethodInfo method)
    {
        Instance = instance;
        Method = method;
    }
}
