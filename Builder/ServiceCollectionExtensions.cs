using Botify.Attributes;
using Botify.Handlers;
using Botify.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Botify.Builder;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBotify(
        this IServiceCollection services,
        Action<BotifyOptionsBuilder> configure)
    {
        services.AddSingleton(sp =>
        {
            var options = new BotifyOptionsBuilder();

            var logger = sp.GetRequiredService<ILogger<BotifyOptionsBuilder>>();
            options.UseLogger(logger);

            configure(options);
            return options;
        });

        services.AddSingleton<BotClientService>();
        services.AddSingleton<LoggerService>();
        services.AddHostedService<BotHostedService>();

        return services;
    }

    public static IServiceCollection AddBotifyHandlers(this IServiceCollection services)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var type in assemblies.SelectMany(a => a.GetTypes()))
        {
            if (!type.IsClass || type.IsAbstract)
                continue;

            if (type.GetCustomAttribute<CommandHandlerAttribute>() != null ||
                type.GetCustomAttribute<CallbackHandlerAttribute>() != null ||
                type.GetCustomAttribute<MessageHandlerAttribute>() != null ||
                type.GetCustomAttribute<InlineHandlerAttribute>() != null)
            {
                services.AddTransient(type);
            }
        }

        services.AddSingleton<CommandHandler>();
        services.AddSingleton<CallbackHandler>();
        services.AddSingleton<MessageHandler>();
        services.AddSingleton<InlineHandler>();

        return services;
    }
}