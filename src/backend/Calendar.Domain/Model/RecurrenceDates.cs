using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Calendar.Domain.Model;

public sealed class RecurrenceDates : IEnumerable<DateTime>, IParsable<RecurrenceDates>
{
    static RecurrenceDates Empty => new([]);
    
    private readonly ImmutableList<DateTime> _values;

    public RecurrenceDates(IEnumerable<DateTime> values)
    {
        _values = [.. values];
    }
    
    public RecurrenceDates Add(DateTime value)
        => new(_values.Add(value)); 
    
    public RecurrenceDates AddRange(IEnumerable<DateTime> values)
        => new(_values.AddRange(values));

    public IEnumerator<DateTime> GetEnumerator()
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
        
        var values = new List<DateTime>();
        foreach (var segment in segments)
        {
            if (!DateTime.TryParse(segment, provider, out var dateTime))
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
