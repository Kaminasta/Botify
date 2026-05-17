using Botify.Attributes;
using Botify.Factories;
using Botify.Handlers;
using Botify.Interfaces;
using Botify.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Botify.Builder;

/// <summary>
/// Методы расширения для регистрации сервисов Botify.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Регистрирует основные сервисы Botify.
    /// </summary>
    /// <param name="services">
    /// Коллекция сервисов приложения.
    /// </param>
    /// <param name="configure">
    /// Делегат конфигурации Botify.
    /// </param>
    /// <returns>
    /// Текущая коллекция сервисов.
    /// </returns>
    public static IServiceCollection AddBotify(
        this IServiceCollection services,
        Action<BotifyOptionsBuilder> configure)
    {
        services.AddSingleton<BotifyOptionsBuilder>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<BotifyOptionsBuilder>>();

            var options = new BotifyOptionsBuilder()
                .UseLogger(logger);

            configure(options);

            return options;
        });

        services.AddSingleton<LoggerService>();
        services.AddSingleton<BotifyContextFactory>();
        services.AddSingleton<BotClientService>();
        services.AddHostedService<BotHostedService>();

        return services;
    }

    /// <summary>
    /// Выполняет автоматическую регистрацию обработчиков Botify.
    /// </summary>
    /// <remarks>
    /// Сканирует загруженные сборки и регистрирует классы,
    /// помеченные handler-атрибутами Botify.
    /// </remarks>
    /// <param name="services">
    /// Коллекция сервисов приложения.
    /// </param>
    /// <returns>
    /// Текущая коллекция сервисов.
    /// </returns>
    public static IServiceCollection AddBotifyHandlers(this IServiceCollection services)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var type in assemblies.SelectMany(a => a.GetTypes()))
        {
            if (!type.IsClass || type.IsAbstract)
                continue;

            if (typeof(IBotifyValidator).IsAssignableFrom(type))
            {
                services.AddTransient(type);
                continue;
            }

            if (type.GetCustomAttribute<CommandHandlerAttribute>() != null ||
                type.GetCustomAttribute<CallbackHandlerAttribute>() != null ||
                type.GetCustomAttribute<MessageHandlerAttribute>() != null ||
                type.GetCustomAttribute<InlineHandlerAttribute>() != null ||
                type.GetCustomAttribute<PaymentHandlerAttribute>() != null
                )
            {
                services.AddTransient(type);
            }
        }

        services.AddSingleton<CommandHandler>();
        services.AddSingleton<CallbackHandler>();
        services.AddSingleton<MessageHandler>();
        services.AddSingleton<InlineHandler>();
        services.AddSingleton<PaymentHandler>();

        return services;
    }
}