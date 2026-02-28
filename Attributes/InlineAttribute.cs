namespace Botify.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class InlineAttribute : Attribute
{
    public string Name { get; }

    public InlineAttribute(string name)
    {
        Name = name.Trim().ToLower();
    }
}