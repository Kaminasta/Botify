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

internal sealed class PaymentHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly BotifyOptionsBuilder _options;
    private readonly LoggerService _logger;

    private readonly List<PaymentInfo> _handlers = new();

    public PaymentHandler(
        IServiceProvider serviceProvider,
        BotifyOptionsBuilder options,
        LoggerService logger)
    {
        _serviceProvider = serviceProvider;
        _options = options;
        _logger = logger;

        LoadHandlers();
    }

    private void LoadHandlers()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies)
        {
            Type[] types;

            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types
                    .Where(t => t != null)
                    .Cast<Type>()
                    .ToArray();
            }

            foreach (var type in types)
            {
                if (!type.IsClass || type.IsAbstract)
                    continue;

                if (type.GetCustomAttribute<PaymentHandlerAttribute>() == null)
                    continue;

                var instance = _serviceProvider.GetRequiredService(type);

                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                {
                    PaymentType? paymentType = null;

                    if (method.GetCustomAttribute<SuccessfulPaymentAttribute>() != null)
                        paymentType = PaymentType.SuccessfulPayment;

                    if (method.GetCustomAttribute<PreCheckoutPaymentAttribute>() != null)
                        paymentType = PaymentType.PreCheckout;

                    if (paymentType == null)
                        continue;

                    ValidateMethodSignature(method);

                    _handlers.Add(CreatePaymentInfo(
                        instance,
                        method,
                        paymentType.Value));

                    _logger.Log(
                        $"Payment handler '{paymentType}' -> {type.FullName}.{method.Name}",
                        LogLevel.Debug);
                }
            }
        }

        _logger.Log(
            $"Loaded {_handlers.Count} payment handlers",
            LogLevel.Debug);
    }

    public async Task<bool> HandleAsync(
        ITelegramBotClient client,
        Update update,
        CancellationToken cancellationToken)
    {
        PaymentType? type = null;

        if (update.Message?.SuccessfulPayment != null)
            type = PaymentType.SuccessfulPayment;
        else if (update.PreCheckoutQuery != null)
            type = PaymentType.PreCheckout;

        if (type == null)
            return false;

        var handlersToInvoke = _handlers
            .Where(h => h.Type == type)
            .ToList();

        if (handlersToInvoke.Count == 0)
            return false;

        var context = new BotifyContext
        {
            Client = client,
            Update = update,
            CancellationToken = cancellationToken,
            Services = _serviceProvider,
            Logger = _logger,
            Options = _options
        };

        foreach (var handler in handlersToInvoke)
            await handler.Delegate(context);

        return true;
    }

    private static void ValidateMethodSignature(MethodInfo method)
    {
        var parameters = method.GetParameters();

        var valid =
            method.ReturnType == typeof(Task) &&
            parameters.Length == 1 &&
            parameters[0].ParameterType == typeof(BotifyContext);

        if (!valid)
            throw new InvalidOperationException(
                $"Method '{method.DeclaringType?.FullName}.{method.Name}' must have signature: Task {method.Name}(BotifyContext context)");
    }

    private static PaymentInfo CreatePaymentInfo(
        object instance,
        MethodInfo method,
        PaymentType paymentType)
    {
        var del = (Func<BotifyContext, Task>)
            Delegate.CreateDelegate(typeof(Func<BotifyContext, Task>), instance, method);

        return new PaymentInfo(del, paymentType);
    }
}