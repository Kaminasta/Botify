namespace Botify.Attributes;

// TODO: Продумать универсальную платежную pipeline-систему

/// <summary>
/// Помечает метод как обработчик события успешной оплаты Telegram.
/// </summary>
/// <remarks>
/// Метод с данным атрибутом вызывается при получении
/// объекта <c>SuccessfulPayment</c> от Telegram.
///
///
/// Атрибут применяется только к методам внутри классов,
/// помеченных атрибутом <see cref="PaymentHandlerAttribute"/>.
///
///
/// Пример:
/// <code>
/// [SuccessfulPayment]
/// public async Task OnPayment(ITelegramBotClient client, Update update, CancellationToken ct)
/// </code>
/// </remarks>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class SuccessfulPaymentAttribute : Attribute
{
}
