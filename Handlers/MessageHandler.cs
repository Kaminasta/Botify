using Botify.Attributes;
using Botify.Factories;
using Botify.Models;
using Botify.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Botify.Handlers;

internal sealed class MessageHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly BotifyOptionsBuilder _options;
    private readonly BotifyContextFactory _contextFactory;
    private readonly LoggerService _logger;

    private readonly List<MessageInfo> _messageHandlers = new();

    public MessageHandler(
        IServiceProvider serviceProvider,
        BotifyOptionsBuilder options,
        BotifyContextFactory contextFactory,
        LoggerService logger)
    {
        _serviceProvider = serviceProvider;
        _options = options;
        _contextFactory = contextFactory;
        _logger = logger;

        LoadMessageHandlers();
    }

    private void LoadMessageHandlers()
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

                if (type.GetCustomAttribute<MessageHandlerAttribute>() == null)
                    continue;

                var instance = _serviceProvider.GetRequiredService(type);

                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);

                foreach (var method in methods)
                {
                    var attr = method.GetCustomAttribute<MessageAttribute>();

                    if (attr == null)
                        continue;

                    if (!BotifyContextFactory.ValidateMethodSignature(method))
                        throw new InvalidOperationException(
                            $"Method '{method.DeclaringType?.FullName}.{method.Name}' must have signature: Task {method.Name}(BotifyContext context)");

                    var info = CreateMessageInfo(instance, method);

                    info.Pattern = attr.Pattern;
                    info.Regex = attr.Regex;
                    info.Types = attr.Types;

                    _messageHandlers.Add(info);

                    _logger.Log(
                        $"Message handler '{attr.Pattern}' ({string.Join(", ", attr.Types)}) -> {type.FullName}.{method.Name}",
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
        var message = update.Message;

        if (message == null)
            return;

        var text =
            !string.IsNullOrWhiteSpace(message.Text)
                ? message.Text
                : !string.IsNullOrWhiteSpace(message.Caption)
                    ? message.Caption
                    : string.Empty;

        foreach (var handler in _messageHandlers)
        {
            if (!handler.Types.Contains(message.Type))
                continue;

            if (!handler.Regex.IsMatch(text))
                continue;

            var context = _contextFactory.Create(client, update, cancellationToken);

            await handler.Delegate(context);

            break;
        }
    }

    private static MessageInfo CreateMessageInfo(
        object instance,
        MethodInfo method)
    {
        var del = (Func<BotifyContext, Task>)
            Delegate.CreateDelegate(typeof(Func<BotifyContext, Task>), instance, method);

        return new MessageInfo(del);
    }
}