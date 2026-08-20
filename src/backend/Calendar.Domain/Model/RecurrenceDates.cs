using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
namespace Calendar.Domain.Model;

[JsonConverter(typeof(ParsableJsonConverter<RecurrenceDates>))]
public sealed class RecurrenceDates : IEnumerable<CalDateTime>, IParsable<RecurrenceDates>
{
    static RecurrenceDates Empty => new([]);
    
    private readonly ImmutableList<CalDateTime> _values;

    // Required by EF Core to materialize this complex type via field access.
    private RecurrenceDates()
    {
        _values = [];
    }

    public RecurrenceDates(IEnumerable<CalDateTime> values)
    {
        _values = [.. values];
    }
    
    public RecurrenceDates Add(CalDateTime value)
        => new(_values.Add(value)); 
    
    public RecurrenceDates AddRange(IEnumerable<CalDateTime> values)
        => new(_values.AddRange(values));

    public IEnumerator<CalDateTime> GetEnumerator()
    {
        return _values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public static RecurrenceDates Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var result) ? result : throw new FormatException();

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out RecurrenceDates result)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            result = Empty;
            return true;
        }
        
        var segments = s.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        
        var values = new List<CalDateTime>();
        foreach (var segment in segments)
        {
            if (!CalDateTime.TryParse(segment, provider, out var dateTime))
            {
                result = Empty;
                return false;
            }
            
            values.Add(dateTime); 
        }

        result = new RecurrenceDates(values);
        return true;
    }
}
