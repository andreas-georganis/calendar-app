using System.Diagnostics.CodeAnalysis;

namespace Calendar.Domain.Model;
public readonly record struct Repeat : IParsable<Repeat>
{
    public int Value { get; }

    public Repeat(int value)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(value, 0, nameof(value));
        Value = value;
    }

    public static Repeat Parse(string s, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Repeat result)
    {
        throw new NotImplementedException();
    }
}
