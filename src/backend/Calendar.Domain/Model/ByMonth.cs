using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Calendar.Domain.Model;

[JsonConverter(typeof(ParsableJsonConverter<ByMonth>))]
public sealed record ByMonth : IParsable<ByMonth>
{
    public IEnumerable<Month> Value { get; }

    public ByMonth(IEnumerable<Month> value)
    {
        Value = value.ToImmutableHashSet();
    }

    public static ByMonth Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var byMonth) ? byMonth : throw new FormatException();

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out ByMonth result)
    {
        var segments = s?.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (segments is null)
        {
            result = default;
            return false;
        }

        try
        {
            result = new ByMonth(segments.Select(segment => Month.Parse(segment, provider)));
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            result = default;
            return false;
        }
    }
}
