using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Calendar.Domain.Model;

public readonly record struct MonthDay : IParsable<MonthDay>
{
    public int Value { get; }

    public MonthDay(int value)
    {
        if (value is 0 || Math.Abs(value) > 31)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }
        
        Value = value;
    }

    public static MonthDay Parse(string s, IFormatProvider? provider)
        => new MonthDay(int.Parse(s, provider));

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out MonthDay result)
    {
        if (!int.TryParse(s, out int value) )
        {
            result = default;
            return false;
        }

        try
        {
            result = new MonthDay(value);
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            result = default;
            return false;
        }
    }
}
