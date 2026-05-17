using Botify.Attributes;
using Botify.Models;
using Botify.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Botify.Handlers;

internal class InlineHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly BotifyOptionsBuilder _options;
    private readonly LoggerService _logger;

    private readonly Dictionary<string, InlineInfo> _inlineMap = new();

    public InlineHandler(
        IServiceProvider serviceProvider,
        BotifyOptionsBuilder options,
        LoggerService logger)
    {
        _serviceProvider = serviceProvider;
        _options = options;
        _logger = logger;

        LoadInlines();
    }

    private void LoadInlines()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsClass)
                    continue;

                if (type.GetCustomAttribute<InlineHandlerAttribute>() == null)
                    continue;

                var instance = _serviceProvider.GetRequiredService(type);

                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);

                foreach (var method in methods)
                {
                    var attr = method.GetCustomAttribute<InlineAttribute>();
                    if (attr == null)
                        continue;

                    var inlineName = attr.Name.ToLower();

                    if (_inlineMap.ContainsKey(inlineName))
                        throw new Exception($"Inline '{inlineName}' already registered");

                    _inlineMap[inlineName] = new InlineInfo(instance!, method);

                    _logger.Log(
                        $"Inline '{inlineName}' -> {type.FullName}.{method.Name}",
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
        var inlineQuery = update.InlineQuery;

        if (inlineQuery == null)
            return;

        var queryText = inlineQuery.Query;

        if (string.IsNullOrWhiteSpace(queryText))
            return;

        var parts = queryText.Split(_options.InlineSplitChar);
        var inline = parts[0].ToLower();

        if (!_inlineMap.TryGetValue(inline, out InlineInfo? inlineInfo))
        {
            _logger.Log(
                $"Неизвестный inline: {inline} от ID: {inlineQuery.From.Id}",
                LogLevel.Debug);
            return;
        }

        var parameters = inlineInfo.Method.GetParameters();
        object?[] args;

        if (parameters.Length == 3)
            args = [client, update, cancellationToken];
        else if (parameters.Length == 2)
            args = [client, update];
        else
            args = Array.Empty<object>();

        var result = inlineInfo.Method.Invoke(inlineInfo.Instance, args);

        if (result is Task task)
            await task;
    }
}