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

internal sealed class InlineHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly BotifyOptionsBuilder _options;
    private readonly BotifyContextFactory _contextFactory;
    private readonly LoggerService _logger;

    private readonly Dictionary<string, InlineInfo> _inlineMap = new();

    public InlineHandler(
        IServiceProvider serviceProvider,
        BotifyOptionsBuilder options,
        BotifyContextFactory contextFactory,
        LoggerService logger)
    {
        _serviceProvider = serviceProvider;
        _options = options;
        _contextFactory = contextFactory;
        _logger = logger;

        LoadInlines();
    }

    private void LoadInlines()
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

                if (type.GetCustomAttribute<InlineHandlerAttribute>() == null)
                    continue;

                var instance = _serviceProvider.GetRequiredService(type);

                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);

                foreach (var method in methods)
                {
                    var attr = method.GetCustomAttribute<InlineAttribute>();

                    if (attr == null)
                        continue;

                    if (!BotifyContextFactory.ValidateMethodSignature(method))
                        throw new InvalidOperationException(
                            $"Method '{method.DeclaringType?.FullName}.{method.Name}' must have signature: Task {method.Name}(BotifyContext context)");

                    var inlineName = attr.Name.ToLowerInvariant();

                    if (!_inlineMap.TryAdd(inlineName, CreateInlineInfo(instance, type, method)))
                        throw new InvalidOperationException($"Inline '{inlineName}' already registered.");

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

        var parts = queryText.Split(
            _options.InlineSplitChar,
            StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 0)
            return;

        var inlineName = parts[0].ToLowerInvariant();

        var context = _contextFactory.Create(client, update, cancellationToken);

        if (!_inlineMap.TryGetValue(inlineName, out var inline))
        {
            _logger.Log(
                $"Unknown inline '{inlineName}' from ID: {inlineQuery.From.Id}",
                LogLevel.Debug);

            if (_options.UnknownInlineHandler != null)
                await _options.UnknownInlineHandler(context);

            return;
        }

        if (!await ValidatorHelper.ValidateAsync(context, inline))
            return;

        await inline.Delegate(context);
    }

    private static InlineInfo CreateInlineInfo(object instance, Type type, MethodInfo method)
    {
        var del = (Func<BotifyContext, Task>)
            Delegate.CreateDelegate(typeof(Func<BotifyContext, Task>), instance, method);

        var validators = ValidatorHelper.GetValidators(type, method);

        return new InlineInfo(del, validators);
    }
}