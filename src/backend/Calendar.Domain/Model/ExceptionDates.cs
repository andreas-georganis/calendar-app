using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
namespace Calendar.Domain.Model;

[JsonConverter(typeof(ParsableJsonConverter<ExceptionDates>))]
public sealed class ExceptionDates : IEnumerable<CalDateTime>, IParsable<ExceptionDates>
{
    static ExceptionDates Empty => new([]);
    
    private readonly ImmutableList<CalDateTime> _values;

    // Required by EF Core to materialize this complex type via field access.
    private ExceptionDates()
    {
        _values = [];
    }

    public ExceptionDates(IEnumerable<CalDateTime> values)
    {
        _values = [.. values];
    }
    
    public ExceptionDates Add(CalDateTime value)
        => new(_values.Add(value));
    
    
    public ExceptionDates AddRange(IEnumerable<CalDateTime> values)
        => new(_values.AddRange(values));
    
    public IEnumerator<CalDateTime> GetEnumerator()
    {
        return _values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public static ExceptionDates Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var result) ? result : throw new FormatException();

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out ExceptionDates result)
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
        
        result = new ExceptionDates(values);
        return true;
    }
}
