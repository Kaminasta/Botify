namespace Botify.Attributes;

/// <summary>
/// Атрибут для метода, обрабатывающего PreCheckout
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class PreCheckoutPaymentAttribute : Attribute { }

