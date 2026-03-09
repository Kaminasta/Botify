using System.Reflection;

namespace Botify.Models;

public class PaymentInfo
{
    public object Instance { get; }
    public MethodInfo Method { get; }
    public PaymentType Type { get; }

    public enum PaymentType
    {
        SuccessfulPayment,
        PreCheckout
    }

    public PaymentInfo(object instance, MethodInfo method, PaymentType type)
    {
        Instance = instance;
        Method = method;
        Type = type;
    }
}
