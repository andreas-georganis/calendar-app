using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Calendar.Domain.Model;

public readonly record struct ByHour : IParsable<ByHour>
{
    public IEnumerable<Hour> Value { get; }

    public ByHour(IEnumerable<Hour> values)
    {
        Value = values.ToImmutableHashSet();
    }

    public static ByHour Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var byHour) ? byHour : throw new FormatException();

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out ByHour result)
    {
        var segments = s?.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (segments is null)
        {
            result = default;
            return false;
        }

        try
        {
            result = new ByHour(segments.Select(segment => Hour.Parse(segment, provider)));
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
           result = default;
           return false;
        }
        
    }
}
