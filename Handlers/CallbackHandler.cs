using Botify.Attributes;
using Botify.Models;
using Botify.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Botify.Handlers;

public class CallbackHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly BotifyOptionsBuilder _options;
    private readonly LoggerService _logger;

    private readonly Dictionary<string, CallbackInfo> _callbackMap = new();

    public CallbackHandler(
        IServiceProvider serviceProvider,
        BotifyOptionsBuilder options,
        LoggerService logger)
    {
        _serviceProvider = serviceProvider;
        _options = options;
        _logger = logger;

        LoadCallbacks();
    }

    private void LoadCallbacks()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsClass)
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

                    var callbackName = attr.Name.ToLower();

                    if (_callbackMap.ContainsKey(callbackName))
                        throw new Exception($"Callback '{callbackName}' already registered");

                    _callbackMap[callbackName] = new CallbackInfo(instance!, method);

                    _logger.Log($"Callback '{callbackName}' -> {type.FullName}.{method.Name}", LogLevel.Debug);
                }
            }
        }
    }

    public async Task HandleAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
    {
        var query = update.CallbackQuery;

        if (query == null)
            return;

        var queryId = query.Id;
        var from = query.From;
        var callbackData = query.Data;

        if (string.IsNullOrEmpty(callbackData))
            return;

        var parts = callbackData.Split(_options.CallbackSplitChar);

        var callback = parts[0].ToLower();

        if (!_callbackMap.TryGetValue(callback, out CallbackInfo? cbInfo))
        {
            _logger.Log($"Неизвестный коллбек: {callback} от ID: {from.Id}", LogLevel.Debug);
            await client.AnswerCallbackQuery(queryId, $"Неизвестный коллбек: {callback}");
            return;
        }

        var parameters = cbInfo.Method.GetParameters();
        object?[] args;

        if (parameters.Length == 3)
            args = [client, update, cancellationToken];
        else if (parameters.Length == 2)
            args = [client, update];
        else
            args = Array.Empty<object>();

        var result = cbInfo.Method.Invoke(cbInfo.Instance, args);

        if (result is Task task)
            await task;
    }
}
