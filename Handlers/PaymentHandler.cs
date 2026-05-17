using Botify.Attributes;
using Botify.Models;
using Botify.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Types;
using static Botify.Models.PaymentInfo;

namespace Botify.Handlers;

internal class PaymentHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly LoggerService _logger;

    private readonly List<PaymentInfo> _handlers = new();

    public PaymentHandler(
        IServiceProvider serviceProvider,
        LoggerService logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        LoadHandlers();
    }

    private void LoadHandlers()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsClass)
                    continue;

                if (type.GetCustomAttribute<PaymentHandlerAttribute>() == null)
                    continue;

                var instance = _serviceProvider.GetRequiredService(type);

                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (method.GetCustomAttribute<SuccessfulPaymentAttribute>() != null)
                        _handlers.Add(new PaymentInfo(instance!, method, PaymentType.SuccessfulPayment));

                    if (method.GetCustomAttribute<PreCheckoutPaymentAttribute>() != null)
                        _handlers.Add(new PaymentInfo(instance!, method, PaymentType.PreCheckout));
                }
            }
        }

        _logger.Log($"Loaded {_handlers.Count} payment handlers", LogLevel.Debug);
    }

    public async Task<bool> HandleAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
    {
        PaymentType? type = null;

        if (update.Message?.SuccessfulPayment != null)
            type = PaymentType.SuccessfulPayment;
        else if (update.PreCheckoutQuery != null)
            type = PaymentType.PreCheckout;

        if (type == null)
            return false;

        var handlersToInvoke = _handlers.Where(h => h.Type == type).ToList();

        foreach (var handler in handlersToInvoke)
        {
            var parameters = handler.Method.GetParameters();
            object?[] args = parameters.Length switch
            {
                3 => [client, update, cancellationToken],
                2 => [client, update],
                _ => Array.Empty<object>()
            };

            var result = handler.Method.Invoke(handler.Instance, args);
            if (result is Task task)
                await task;
        }

        return true;
    }
}
