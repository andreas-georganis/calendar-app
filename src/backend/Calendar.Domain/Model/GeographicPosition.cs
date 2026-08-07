using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Calendar.Domain.Model;

[JsonConverter(typeof(ParsableJsonConverter<GeographicPosition>))]
public readonly record struct GeographicPosition : IParsable<GeographicPosition>
{
    public required double Latitude { get; init; }
    
    public required double Longitude { get; init; }

    public static GeographicPosition Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var result) ? result: throw new FormatException();

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out GeographicPosition result)
    {
        var segments = s?.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (segments is null)
        {
            result = default;
            return false;
        }

        var parseResult = double.TryParse(segments[0], out var latitude) & double.TryParse(segments[1], out var longtitude);

        if (parseResult is false)
        {
            result = default;
            return false;
        }

        result = new()
        {
            Latitude = latitude,
            Longitude = longtitude
        };
        return true;
    }
}
