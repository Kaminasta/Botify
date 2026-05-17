using System.Reflection;

namespace Botify.Models;

internal class CommandInfo
{
    public object Instance { get; }
    public MethodInfo Method { get; }

    public CommandInfo(object instance, MethodInfo method)
    {
        Instance = instance;
        Method = method;
    }
}
