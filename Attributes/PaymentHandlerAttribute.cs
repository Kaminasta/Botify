namespace Botify.Attributes;

// TODO: Продумать универсальную платежную pipeline-систему

/// <summary>
/// Помечает класс как контейнер обработчиков Telegram-платежей.
/// </summary>
/// <remarks>
/// Классы с данным атрибутом автоматически сканируются Botify
/// для поиска методов, помеченных:
/// <list type="bullet">
/// <item>
/// <description><see cref="PreCheckoutPaymentAttribute"/></description>
/// </item>
/// <item>
/// <description><see cref="SuccessfulPaymentAttribute"/></description>
/// </item>
/// </list>
///
/// Используется для группировки логики,
/// связанной с Telegram Payments.
///
/// Пример:
/// <code>
/// [PaymentHandler]
/// public class DonatePayments
/// {
///     [PreCheckoutPayment]
///     public async Task PreCheckout(ITelegramBotClient client, Update update, CancellationToken ct)
///
///     [SuccessfulPayment]
///     public async Task Successful(ITelegramBotClient client, Update update, CancellationToken ct)
/// }
/// </code>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class PaymentHandlerAttribute : Attribute
{
}