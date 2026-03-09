namespace Botify.Attributes;

/// <summary>
/// Атрибут для класса-обработчика платежей
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class PaymentHandlerAttribute : Attribute { }
