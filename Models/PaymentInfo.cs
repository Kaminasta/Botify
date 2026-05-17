using System.Reflection;

namespace Botify.Models;

internal class PaymentInfo
{
    public Func<BotifyContext, Task> Delegate { get; }
    public PaymentType Type { get; }

    public enum PaymentType
    {
        SuccessfulPayment,
        PreCheckout
    }

    public PaymentInfo(Func<BotifyContext, Task> @delegate, PaymentType paymentType)
    {
        Delegate = @delegate;
        Type = paymentType;
    }
}
