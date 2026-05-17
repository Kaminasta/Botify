using Botify.Enums;

namespace Botify.Models;

internal sealed class PaymentInfo(
    Func<BotifyContext, Task> @delegate, 
    PaymentType paymentType)
{
    public Func<BotifyContext, Task> Delegate { get; } = @delegate;
    public PaymentType Type { get; } = paymentType;
}
