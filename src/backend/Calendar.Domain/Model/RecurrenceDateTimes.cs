using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Calendar.Domain.Model;

public sealed class RecurrenceDateTimes : IEnumerable<Period>, IParsable<RecurrenceDateTimes>
{
    static RecurrenceDateTimes Empty => new RecurrenceDateTimes(ImmutableList<Period>.Empty);
    
    private readonly ImmutableList<Period> _values;

    public RecurrenceDateTimes(IEnumerable<Period> values)
    {
        _values = values.ToImmutableList();
    }
    
    public RecurrenceDateTimes Add(Period value)
        => new RecurrenceDateTimes(_values.Add(value)); 
    
    public RecurrenceDateTimes AddRange(IEnumerable<Period> values)
        => new RecurrenceDateTimes(_values.AddRange(values));

    public IEnumerator<Period> GetEnumerator()
    {
        return _values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public static RecurrenceDateTimes Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var result) ? result : throw new FormatException();

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out RecurrenceDateTimes result)
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

        result = new RecurrenceDateTimes(values);
        return true;
    }
}
