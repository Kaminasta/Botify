namespace Botify.Attributes;

/// <summary>
/// Атрибут для метода, обрабатывающего успешную оплату (SuccessfulPayment)
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class SuccessfulPaymentAttribute : Attribute { }