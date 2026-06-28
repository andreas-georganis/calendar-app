using System.Diagnostics.CodeAnalysis;

namespace Calendar.Domain.Model;

public readonly record struct Second: IParsable<Second>
{
    public int Value { get; }

    public Second(int value)
    {
        if (value is < 0 or > 60)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }
        
        Value = value;
    }

    public static Second Parse(string s, IFormatProvider? provider)
        => new Second(int.Parse(s, provider));

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Second result)
    {
        if (!int.TryParse(s, out int value) )
        {
            result = default;
            return false;
        }

        try
        {
            result = new Second(value);
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            result = default;
            return false;
        }
    }
}
