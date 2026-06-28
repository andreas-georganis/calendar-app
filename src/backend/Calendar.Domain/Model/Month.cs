using System.Diagnostics.CodeAnalysis;

namespace Calendar.Domain.Model;

public readonly record struct Month : IParsable<Month>
{
    public Month(int value)
    {
        if (value is < 0 or > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }
       
        Value = value;
    }

    public int Value { get; }

    public static Month Parse(string s, IFormatProvider? provider)
        => new Month(int.Parse(s, provider));

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Month result)
    {
        if (!int.TryParse(s, provider, out int value))
        {
            result = default;
            return false;
        }

        try
        {
            result = new Month(value);
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            result = default;
            return false;
        }
        
    }
}
