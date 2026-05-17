using Botify.Attributes;
using Botify.Factories;
using Botify.Models;
using Botify.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Botify.Handlers;

internal sealed class CommandHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly BotClientService _botClient;
    private readonly BotifyOptionsBuilder _options;
    private readonly BotifyContextFactory _contextFactory;
    private readonly LoggerService _logger;

    private readonly Dictionary<string, CommandInfo> _commandMap = new();
    private readonly List<BotCommand> _telegramCommands = new();

    public CommandHandler(
        IServiceProvider serviceProvider,
        BotClientService botClient,
        BotifyOptionsBuilder options,
        BotifyContextFactory contextFactory,
        LoggerService logger)
    {
        _serviceProvider = serviceProvider;
        _botClient = botClient;
        _options = options;
        _contextFactory = contextFactory;
        _logger = logger;

        LoadCommands();
    }

    private void LoadCommands()
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

                if (type.GetCustomAttribute<CommandHandlerAttribute>() == null)
                    continue;

                var instance = _serviceProvider.GetRequiredService(type);
                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);

                foreach (var method in methods)
                {
                    var attr = method.GetCustomAttribute<CommandAttribute>();
                    if (attr == null)
                        continue;

                    if (!BotifyContextFactory.ValidateMethodSignature(method))
                        throw new InvalidOperationException(
                            $"Method '{method.DeclaringType?.FullName}.{method.Name}' must have signature: Task {method.Name}(BotifyContext context)");

                    var commandName = attr.Name.ToLowerInvariant();

                    if (!_commandMap.TryAdd(commandName, CreateCommandInfo(instance, type, method)))
                        throw new InvalidOperationException($"Command '{commandName}' already registered.");

                    if (!string.IsNullOrWhiteSpace(attr.Description))
                        _telegramCommands.Add(new BotCommand(commandName, attr.Description));

                    _logger.Log(
                        $"Command '{commandName}' -> {type.FullName}.{method.Name}",
                        LogLevel.Debug);
                }
            }
        }
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (_telegramCommands.Count == 0)
            return;

        await _botClient.Client.SetMyCommands(
            _telegramCommands,
            cancellationToken: cancellationToken);
    }

    public async Task<bool> HandleAsync(
        ITelegramBotClient client,
        Update update,
        CancellationToken cancellationToken)
    {
        var message = update.Message;

        if (message == null || message.Type != MessageType.Text)
            return false;

        var text = message.Text;
        if (string.IsNullOrWhiteSpace(text))
            return false;

        if (!text.StartsWith(_options.CommandStartChar))
            return false;

        var parts = text.Substring(1).Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            return false;

        var commandName = parts[0].Split('@', 2)[0].ToLowerInvariant();

        var context = _contextFactory.Create(client, update, cancellationToken);

        if (!_commandMap.TryGetValue(commandName, out var command))
        {
            _logger.Log($"Unknown command '{commandName}' from ID: {message.From?.Id}", LogLevel.Debug);

            if (_options.UnknownCommandHandler != null)
                await _options.UnknownCommandHandler(context);

            return false;
        }

        if (!await ValidatorHelper.ValidateAsync(context, command))
            return true;

        await command.Delegate(context);
        return true;
    }

    private static CommandInfo CreateCommandInfo(object instance, Type type, MethodInfo method)
    {
        var del = (Func<BotifyContext, Task>)
            Delegate.CreateDelegate(typeof(Func<BotifyContext, Task>), instance, method);

        var validators = ValidatorHelper.GetValidators(type, method);

        return new CommandInfo(del, validators);
    }
}