using System.Reflection;

namespace Botify.Models;

public class InlineInfo
{
    public object Instance { get; }
    public MethodInfo Method { get; }

    public InlineInfo(object instance, MethodInfo method)
    {
        Instance = instance;
        Method = method;
    }
}