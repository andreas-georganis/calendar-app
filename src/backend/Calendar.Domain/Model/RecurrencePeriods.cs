using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Calendar.Domain.Model;

public sealed class RecurrencePeriods : IEnumerable<Period>, IParsable<RecurrencePeriods>
{
    static RecurrencePeriods Empty => new(ImmutableList<Period>.Empty);
    
    private readonly ImmutableList<Period> _values;

    public RecurrencePeriods(IEnumerable<Period> values)
    {
        _values = [.. values];
    }
    
    public RecurrencePeriods Add(Period value)
        => new(_values.Add(value)); 
    
    public RecurrencePeriods AddRange(IEnumerable<Period> values)
        => new(_values.AddRange(values));

    public IEnumerator<Period> GetEnumerator()
    {
        return _values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public static RecurrencePeriods Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var result) ? result : throw new FormatException();

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out RecurrencePeriods result)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            result = Empty;
            return true;
        }
        
        var segments = s.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        
        var values = new List<Period>();
        foreach (var segment in segments)
        {
            if (!Period.TryParse(segment, provider, out var period))
            {
                result = Empty;
                return false;
            }
            
            values.Add(period); 
        }

        result = new RecurrencePeriods(values);
        return true;
    }
}
