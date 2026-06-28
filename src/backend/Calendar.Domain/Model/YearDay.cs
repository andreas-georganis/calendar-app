using System.Diagnostics.CodeAnalysis;

namespace Calendar.Domain.Model;

public readonly record struct YearDay : IParsable<YearDay>
{
    public YearDay(int value)
    {
        if (value is 0 || Math.Abs(value) > 366)
            throw new ArgumentOutOfRangeException(nameof(value));

        Value = value;
    }

    public int Value { get; }
    public static YearDay Parse(string s, IFormatProvider? provider)
        => new YearDay(int.Parse(s, provider));

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out YearDay result)
    {
        if (!int.TryParse(s, provider, out var value))
        {
            result = default;
            return false;
        }

        try
        {
            result = new YearDay(value);
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            result = default;
            return false;
        }
    }
}
