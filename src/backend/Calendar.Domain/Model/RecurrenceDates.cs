using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Calendar.Domain.Model;

file interface IRecurrenceDates<out T> : IEnumerable<T>;

public sealed class RecurrenceDates
{
    public RecurrenceDates(RecurrenceDateTimes dateTimes, RecurrencePeriods periods)
    {
        
    }
}

public sealed class RecurrenceDateTimes : IRecurrenceDates<DateTime>, IParsable<RecurrenceDateTimes>
{
    static RecurrenceDateTimes Empty => new RecurrenceDateTimes(ImmutableList<DateTime>.Empty);
    private readonly ImmutableList<DateTime> _values;
    
    public RecurrenceDateTimes(IEnumerable<DateTime> values)
    {
        _values = values.ToImmutableList();
    }
    
    public RecurrenceDateTimes Add(DateTime value)
        => new RecurrenceDateTimes(_values.Add(value)); 
    
    public RecurrenceDateTimes AddRange(IEnumerable<DateTime> values)
        => new RecurrenceDateTimes(_values.AddRange(values));
    
    public IEnumerator<DateTime> GetEnumerator()
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
        
        result = new RecurrenceDateTimes(values);
        return true;
    }
}



public sealed class RecurrencePeriods : IEnumerable<Period>, IParsable<RecurrencePeriods>
{
    static RecurrencePeriods Empty => new RecurrencePeriods(ImmutableList<Period>.Empty);
    
    private readonly ImmutableList<Period> _values;

    public RecurrencePeriods(IEnumerable<Period> values)
    {
        _values = values.ToImmutableList();
    }
    
    public RecurrencePeriods Add(Period value)
        => new RecurrencePeriods(_values.Add(value)); 
    
    public RecurrencePeriods AddRange(IEnumerable<Period> values)
        => new RecurrencePeriods(_values.AddRange(values));

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
