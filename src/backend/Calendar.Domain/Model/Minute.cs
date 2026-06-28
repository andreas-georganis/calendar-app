using System.Diagnostics.CodeAnalysis;

namespace Calendar.Domain.Model;

public readonly record struct Minute: IParsable<Minute>
{
    public int Value { get; }

    public Minute(int value)
    {
        if (value is < 0 or > 59)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }
        
        Value = value;
    }

    public static Minute Parse(string s, IFormatProvider? provider)
        => new Minute(int.Parse(s, provider));

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Minute result)
    {
        if (!int.TryParse(s, out int value) )
        {
            result = default;
            return false;
        }

        try
        {
            result = new Minute(value);
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            result = default;
            return false;
        }
    }
}
