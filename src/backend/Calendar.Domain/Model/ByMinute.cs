using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Calendar.Domain.Model;

public readonly record struct ByMinute : IParsable<ByMinute>
{
    public IEnumerable<Minute> Value { get; }

    public ByMinute(IEnumerable<Minute> values)
    {
        Value = values.ToImmutableHashSet();
    }

    public static ByMinute Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var byMinute) ? byMinute : throw new FormatException();

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out ByMinute result)
    {
        var segments = s?.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (segments is null)
        {
            result = default;
            return false;
        }

        try
        {
            result = new ByMinute(segments.Select(segment => Minute.Parse(segment, provider)));
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            result = default;
            return false;
        }
    }
}
