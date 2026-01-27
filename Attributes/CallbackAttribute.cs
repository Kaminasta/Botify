namespace Botify.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class CallbackAttribute : Attribute
{
    public string Name { get; internal set; } = string.Empty;

    public CallbackAttribute(string callback)
    {
        SetCallbackName(callback);
    }
    private void SetCallbackName(string callback)
    {
        if (string.IsNullOrWhiteSpace(callback))
            throw new ArgumentException("Callback cannot be empty", nameof(callback));

        Name = callback.Trim().ToLower();

        if (callback.Contains(" "))
            throw new ArgumentException("Callback cannot contain spaces", nameof(callback));

        Name = callback;
    }
}
