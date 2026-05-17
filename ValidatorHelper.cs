using Botify.Attributes;
using Botify.Interfaces;
using Botify.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Botify;

internal static class ValidatorHelper
{
    public static IReadOnlyList<UseValidatorAttribute> GetValidators(Type type, MethodInfo method)
    {
        var validators = new List<UseValidatorAttribute>();

        validators.AddRange(
            type.GetCustomAttributes<UseValidatorAttribute>(true));

        validators.AddRange(
            method.GetCustomAttributes<UseValidatorAttribute>(true));

        return validators
            // .OrderBy(v => v.Order) // TODO: Дальше нужно будет добавить порядок
            .ToList();
    }

    public static async Task<bool> ValidateAsync(BotifyContext context, IValidatable command)
    {
        foreach (var attr in command.Validators)
        {
            var validator =
                (IBotifyValidator)context.Services.GetRequiredService(attr.ValidatorType);

            bool result = await validator.ValidateAsync(context);

            if (!result)
                return false;
        }

        return true;
    }
}
