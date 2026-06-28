using System.Diagnostics.CodeAnalysis;

namespace Calendar.Domain.Model;

public readonly record struct UntilOrCount : IParsable<UntilOrCount>
{
    public static UntilOrCount Forever => new(null, null);

    public static UntilOrCount On(DateTime value)
    {
        return new(value, null);
    }

    public static UntilOrCount After(Count value)
        => new(null, value);
    
    private UntilOrCount(DateTime? until, Count? count)
    {
        Until = until;
        Count = count;
    }

    public DateTime? Until { get; }
    public Count? Count { get; }
    
    public static UntilOrCount Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var result) ? result: throw new FormatException();

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out UntilOrCount result)
    {
        DateTime.TryParse(s, provider, out var until);
        if (until != default)
        {
            result = new(until, null);
            return true;
        }

        Model.Count.TryParse(s, provider, out var count);
        if (count != default)
        {
            result = new(null, count);
            return true;
        }

        result = default;
        return false;
    }
}
