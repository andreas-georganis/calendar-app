using System.Diagnostics.CodeAnalysis;

namespace Calendar.Domain.Model;

public readonly record struct SetPos : IParsable<SetPos>
{
    public SetPos(int value)
    {
        if (value is 0 || Math.Abs(value) > 366)
            throw new ArgumentOutOfRangeException(nameof(value));

        Value = value;
    }

    public int Value { get;  }

    public static SetPos Parse(string s, IFormatProvider? provider)
        => new SetPos(int.Parse(s, provider));

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out SetPos result)
    {
        if (!int.TryParse(s, out int value) )
        {
            result = default;
            return false;
        }

        try
        {
            result = new SetPos(value);
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            result = default;
            return false;
        }
    }
}
