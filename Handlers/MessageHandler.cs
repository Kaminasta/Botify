using Botify.Attributes;
using Botify.Models;
using Botify.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Botify.Handlers;

internal class MessageHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly LoggerService _logger;

    private readonly List<MessageInfo> _messageHandlers = new();

    public MessageHandler(IServiceProvider serviceProvider, LoggerService logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        LoadMessageHandlers();
    }

    private void LoadMessageHandlers()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsClass)
                    continue;

                if (type.GetCustomAttribute<MessageHandlerAttribute>() == null)
                    continue;

                var instance = _serviceProvider.GetRequiredService(type);

                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);

                foreach (var method in methods)
                {
                    var attr = method.GetCustomAttribute<MessageAttribute>();
                    if (attr == null)
                        continue;

                    _messageHandlers.Add(new MessageInfo
                    {
                        Instance = instance!,
                        Method = method,
                        Pattern = attr.Pattern,
                        Regex = attr.Regex,
                        Types = attr.Types
                    });

                    _logger.Log($"Message handler '{attr.Pattern}' ({string.Join(", ", attr.Types)}) -> {type.FullName}.{method.Name}", LogLevel.Debug);
                }
            }
        }
    }

    public async Task HandleAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
    {
        var message = update.Message;

        if (message == null)
            return;

        var text = 
            !string.IsNullOrEmpty(message.Text) ? message.Text :
            !string.IsNullOrEmpty(message.Caption) ? message.Caption : string.Empty;

        foreach (var handler in _messageHandlers)
        {
            if (!handler.Types.Contains(message.Type))
                continue;

            if (handler.Regex.IsMatch(text))
            {
                var parameters = handler.Method.GetParameters();
                object?[] args;

                if (parameters.Length == 3)
                    args = [client, update, cancellationToken];
                else if (parameters.Length == 2)
                    args = [client, update];
                else
                    args = Array.Empty<object>();

                var result = handler.Method.Invoke(handler.Instance, args);

                if (result is Task task)
                    await task;

                break;
            }
        }
    }
}
