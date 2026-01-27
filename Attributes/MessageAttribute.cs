using System.Text.RegularExpressions;

namespace Botify.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class MessageAttribute : Attribute
{
    public string Pattern { get; }
    public Regex Regex { get; }

    public MessageAttribute(string pattern)
    {
        Pattern = pattern;
        Regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }

    public MessageAttribute(string pattern, RegexOptions regexOptions)
    {
        Pattern = pattern;
        Regex = new Regex(pattern, regexOptions | RegexOptions.Compiled);
    }
}
