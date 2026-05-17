using Botify.Interfaces;

namespace Botify.Attributes;

/// <summary>
/// Подключает валидатор к методу-обработчику
/// или ко всему классу обработчика.
/// </summary>
/// <remarks>
/// Атрибут используется для выполнения предварительных проверок
/// перед вызовом обработчика Telegram-запроса.
///
/// Валидатор должен реализовывать интерфейс
/// <see cref="IBotifyValidator"/>.
///
/// Атрибут может быть применён:
/// <list type="bullet">
/// <item>
/// К отдельному методу — валидатор будет вызван
/// только для данного обработчика.
/// </item>
/// <item>
/// К классу обработчика — валидатор будет вызван
/// для всех методов данного класса.
/// </item>
/// </list>
///
///
/// Пример:
/// <code>
/// [UseValidator(typeof(AdminValidator))]
/// [UseValidator(typeof(SubscriptionValidator))]
/// public async Task AdminCommand(BotifyContext context)
/// </code>
/// </remarks>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public sealed class UseValidatorAttribute : Attribute
{
    /// <summary>
    /// Тип валидатора.
    /// </summary>
    /// <remarks>
    /// Тип должен реализовывать интерфейс
    /// <see cref="IBotifyValidator"/>.
    /// </remarks>
    public Type ValidatorType { get; }

    /// <summary>
    /// Создаёт атрибут подключения валидатора.
    /// </summary>
    /// <param name="validatorType">
    /// Тип валидатора,
    /// реализующего <see cref="IBotifyValidator"/>.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Выбрасывается,
    /// если указанный тип не реализует
    /// интерфейс <see cref="IBotifyValidator"/>.
    /// </exception>
    public UseValidatorAttribute(Type validatorType)
    {
        if (!typeof(IBotifyValidator).IsAssignableFrom(validatorType))
            throw new InvalidOperationException($"{validatorType.Name} must implement IBotifyValidator");

        ValidatorType = validatorType;
    }
}