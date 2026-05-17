using Botify.Attributes;
using Botify.Models;
using Botify.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Botify.Handlers;

internal class CommandHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly BotClientService _botClient;
    private readonly BotifyOptionsBuilder _options;
    private readonly LoggerService _logger;

    private readonly Dictionary<string, CommandInfo> _commandMap = new();

    public CommandHandler(
        IServiceProvider serviceProvider, 
        BotClientService botClient, 
        BotifyOptionsBuilder options, 
        LoggerService logger)
    {
        _serviceProvider = serviceProvider;
        _botClient = botClient;
        _options = options;
        _logger = logger;

        LoadCommands();
    }

    private async void LoadCommands()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var telegramCommands = new List<BotCommand>();

        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsClass)
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

                    var commandName = attr.Name.ToLower();
                    var commandDescription = attr.Description;

                    if (_commandMap.ContainsKey(commandName))
                        throw new Exception($"Command '{commandName}' already registered");

                    _commandMap[commandName] = new CommandInfo(instance!, method);

                    if(!string.IsNullOrEmpty(commandDescription))
                        telegramCommands.Add(new BotCommand(commandName, commandDescription));

                    _logger.Log($"Command '{commandName}' -> {type.FullName}.{method.Name}", LogLevel.Debug);
                }
            }
        }

        await _botClient.Client.SetMyCommands(telegramCommands);
    }

    public async Task<bool> HandleAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
    {
        var message = update.Message;
        if (message == null || message.Type != MessageType.Text)
            return false;

        var text = message.Text!;
        if (!text.StartsWith(_options.CommandStartChar))
            return false;

        var parts = text.Substring(1).Split(' ');
        var command = parts[0].ToLower();

        if (!_commandMap.TryGetValue(command, out CommandInfo? cmdInfo))
        {
            _logger.Log($"Неизвестная команда: {command}", LogLevel.Debug);
            await _botClient.Client.SendMessage(message.Chat.Id, $"Неизвестная команда: {command}");
            return false;
        }

        var parameters = cmdInfo.Method.GetParameters();
        object?[] args;

        if (parameters.Length == 3)
            args = [client, update, cancellationToken];
        else if (parameters.Length == 2)
            args = [client, update];
        else
            args = Array.Empty<object>();

        var result = cmdInfo.Method.Invoke(cmdInfo.Instance, args);

        if (result is Task task)
            await task;

        return true;
    }
}
