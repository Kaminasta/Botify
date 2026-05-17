using Botify.Attributes;
using Botify.Factories;
using Botify.Interfaces;
using Botify.Models;
using Botify.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Botify.Handlers;

internal sealed class CallbackHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly BotifyOptionsBuilder _options;
    private readonly BotifyContextFactory _contextFactory;
    private readonly LoggerService _logger;

    private readonly Dictionary<string, CallbackInfo> _callbackMap = new();

    public CallbackHandler(
        IServiceProvider serviceProvider,
        BotifyOptionsBuilder options,
        BotifyContextFactory contextFactory,
        LoggerService logger)
    {
        _serviceProvider = serviceProvider;
        _options = options;
        _contextFactory = contextFactory;
        _logger = logger;

        LoadCallbacks();
    }

    private void LoadCallbacks()
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

                if (type.GetCustomAttribute<CallbackHandlerAttribute>() == null)
                    continue;

                var instance = _serviceProvider.GetRequiredService(type);

                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);

                foreach (var method in methods)
                {
                    var attr = method.GetCustomAttribute<CallbackAttribute>();

                    if (attr == null)
                        continue;

                    if (!BotifyContextFactory.ValidateMethodSignature(method))
                        throw new InvalidOperationException(
                            $"Method '{method.DeclaringType?.FullName}.{method.Name}' must have signature: Task {method.Name}(BotifyContext context)");

                    var callbackName = attr.Name.ToLowerInvariant();

                    if (!_callbackMap.TryAdd(callbackName, CreateCallbackInfo(instance, type, method)))
                        throw new InvalidOperationException($"Callback '{callbackName}' already registered.");

                    _logger.Log(
                        $"Callback '{callbackName}' -> {type.FullName}.{method.Name}",
                        LogLevel.Debug);
                }
            }
        }
    }

    public async Task HandleAsync(
        ITelegramBotClient client,
        Update update,
        CancellationToken cancellationToken)
    {
        var query = update.CallbackQuery;

        if (query?.Data == null)
            return;

        var parts = query.Data.Split(_options.CallbackSplitChar);

        if (parts.Length == 0)
            return;

        var callbackName = parts[0].ToLowerInvariant();

        var context = _contextFactory.Create(client, update, cancellationToken);

        if (!_callbackMap.TryGetValue(callbackName, out var callback))
        {
            _logger.Log(
                $"Unknown callback '{callbackName}' from ID: {query.From.Id}",
                LogLevel.Debug);

            if (_options.UnknownCallbackHandler != null)
                await _options.UnknownCallbackHandler(context);

            return;
        }

        if (!await ValidatorHelper.ValidateAsync(context, callback))
            return;

        await callback.Delegate(context);
    }

    private static CallbackInfo CreateCallbackInfo(object instance, Type type, MethodInfo method)
    {
        var del = (Func<BotifyContext, Task>)
            Delegate.CreateDelegate(typeof(Func<BotifyContext, Task>), instance, method);

        var validators = ValidatorHelper.GetValidators(type, method);

        return new CallbackInfo(del, validators);
    }
}