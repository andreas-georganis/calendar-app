using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Calendar.Domain.Model;

[JsonConverter(typeof(ParsableJsonConverter<BySecond>))]
public sealed record BySecond : IParsable<BySecond>
{
    public IEnumerable<Second> Value { get; }

    public BySecond(IEnumerable<Second> value)
    {
        Value = value.ToImmutableHashSet();
    }

    public static BySecond Parse(string s, IFormatProvider? provider) 
        => TryParse(s, provider, out var result) ? result : throw new FormatException();

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [NotNullWhen(true)] out BySecond? result)
    {
        var segments = s?.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (segments is null)
        {
            result = default;
            return false;
        }

        try
        {
            result = new BySecond(segments.Select(segment => Second.Parse(segment, provider)));
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            result = default;
            return false;
        }
    }
}
