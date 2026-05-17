using Botify.Services;
using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Botify.Factories;

internal sealed class BotifyContextFactory
{
    private readonly IServiceProvider _services;
    private readonly LoggerService _logger;
    private readonly BotifyOptionsBuilder _options;

    public BotifyContextFactory(
        IServiceProvider services,
        LoggerService logger,
        BotifyOptionsBuilder options)
    {
        _services = services;
        _logger = logger;
        _options = options;
    }

    public BotifyContext Create(
        ITelegramBotClient client,
        Update update,
        CancellationToken cancellationToken)
    {
        return new BotifyContext
        {
            Client = client,
            Update = update,
            CancellationToken = cancellationToken,
            Services = _services,
            Logger = _logger,
            Options = _options
        };
    }

    public static bool ValidateMethodSignature(MethodInfo method)
    {
        var parameters = method.GetParameters();

        var valid =
            method.ReturnType == typeof(Task) &&
            parameters.Length == 1 &&
            parameters[0].ParameterType == typeof(BotifyContext);

        return valid;
    }
}