using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Calendar.Domain.Model;

public readonly record struct ByDay : IParsable<ByDay>
{
    public IEnumerable<OrdinalWeekDay> Values { get; }

    public ByDay(IEnumerable<OrdinalWeekDay> values)
    {
        Values = values.ToImmutableHashSet();
    }

    public static ByDay Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var result) ? result : throw new FormatException();

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out ByDay result)
    {
        var segments = s?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        
        if (segments is null)
        {
            result = default;
            return false;
        }

        try
        {
            var values = segments.Select(x=>OrdinalWeekDay.Parse(x, provider));
            
            result = new ByDay(values);
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
           result = default;
           return false;
        }
    }
}
