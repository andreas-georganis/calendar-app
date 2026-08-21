using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Calendar.Domain.Model;

[JsonConverter(typeof(ParsableJsonConverter<ByYearDay>))]
public sealed record ByYearDay : IParsable<ByYearDay>
{
    public IEnumerable<YearDay> Value { get; }
    
    public ByYearDay(IEnumerable<YearDay> value)
    {
        Value = value.ToImmutableHashSet();
    }

    public static ByYearDay Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var result) ? result : throw new FormatException();

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [NotNullWhen(true)] out ByYearDay? result)
    {
        var segments = s?.Split(',', StringSplitOptions.RemoveEmptyEntries 
                                     | StringSplitOptions.TrimEntries);
        
        if (segments is null or { Length:0 })
        {
            result = default;
            return false;
        }

        var yearDays = new List<YearDay>();
        foreach (var segment in segments)
        {
            if (!YearDay.TryParse(segment, provider, out var yearDay))
            {
                result = default;
                return false;
            }
            yearDays.Add(yearDay);
        }

        result = new ByYearDay(yearDays);
        return true;
    }
}
