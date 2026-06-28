using System.Diagnostics.CodeAnalysis;

namespace Calendar.Domain.Model;

public readonly record struct Count : IParsable<Count>
{
    public int Value { get; }

    public Count(int value)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(value,1);
        Value = value;
    }

    public static Count Parse(string s, IFormatProvider? provider)
        => new Count(int.Parse(s, provider));

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Count result)
    {
        if (!int.TryParse(s, provider, out int value))
        {
            result = default;
            return false;
        }

        try
        {
            result = new Count(value);
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            result = default;
            return false;
        }

        
    }
}

