namespace Botify.Attributes;

// TODO: Продумать универсальную платежную pipeline-систему

/// <summary>
/// Помечает метод как обработчик события предварительной проверки платежа Telegram.
/// </summary>
/// <remarks>
/// Метод с данным атрибутом вызывается при получении
/// события <c>PreCheckoutQuery</c>.
///
/// Telegram ожидает подтверждение возможности проведения платежа.
/// Если запрос не будет подтверждён,
/// пользователь не сможет завершить оплату.
///
/// Атрибут применяется только к методам внутри классов,
/// помеченных атрибутом <see cref="PaymentHandlerAttribute"/>.
///
/// Пример:
/// <code>
/// [PreCheckoutPayment]
/// public async Task OnPreCheckout(BotifyContext context)
/// </code>
/// </remarks>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class PreCheckoutPaymentAttribute : Attribute
{
}