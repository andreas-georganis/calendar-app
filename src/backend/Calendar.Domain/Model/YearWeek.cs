using System.Diagnostics.CodeAnalysis;

namespace Calendar.Domain.Model;

public readonly record struct YearWeek : IParsable<YearWeek>
{
    public YearWeek(int value)
    {
        if (value is 0 || Math.Abs(value) > 53)
            throw new ArgumentOutOfRangeException(nameof(value));
        
        Value = value;
    }
    
    public int Value { get; }
    
    public static implicit operator int(YearWeek yearWeek) => yearWeek.Value;

    public static YearWeek Parse(string s, IFormatProvider? provider)
        => new YearWeek(int.Parse(s,  provider));

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out YearWeek result)
    {
        if (!int.TryParse(s, provider, out var value))
        {
            result = default;
            return false;
        }

        try
        {
            result = new YearWeek(value);
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            result = default;
            return false;
        }
        
    }
}
