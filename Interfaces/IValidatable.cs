using Botify.Attributes;

namespace Botify.Interfaces;

internal interface IValidatable
{
    IReadOnlyList<UseValidatorAttribute> Validators { get; }
}
