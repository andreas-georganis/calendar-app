using System.Diagnostics.CodeAnalysis;

namespace Calendar.Domain.Model;

public readonly record struct Interval : IParsable<Interval>
{
    public static Interval One() => new(1);
    
    public int Value { get; }

    public Interval(int value)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(value,1);
        Value = value;
    }

    public static Interval Parse(string s, IFormatProvider? provider)
        => new Interval(int.Parse(s, provider));

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Interval result)
    {
        if (!int.TryParse(s, provider, out var value))
        {
            result = default;
            return false;
        }

        try
        {
            result = new Interval(value);
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            result = default;
            return false;
        }
        
    }
}

