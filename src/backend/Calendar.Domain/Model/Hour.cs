using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Calendar.Domain.Model;


public readonly record struct Hour : IParsable<Hour> 
{
    public int Value { get; }

    private Hour(int value)
    {
        if (value is < 0 or > 23)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }
        
        Value = value;
    }
    
    public static Hour Parse(string s, IFormatProvider? provider)
        => new Hour(int.Parse(s, provider));

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Hour result)
    {
        if (!int.TryParse(s, provider, out int value))
        {
            result = default;
            return false;
        }

        try
        {
            result = new Hour(value);
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            result = default;
            return false;
        }
    }
}
