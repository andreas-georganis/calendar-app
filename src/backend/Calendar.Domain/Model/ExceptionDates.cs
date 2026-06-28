using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Calendar.Domain.Model;

public sealed class ExceptionDates : IEnumerable<DateTime>, IParsable<ExceptionDates>
{
    static ExceptionDates Empty => new ExceptionDates(ImmutableList<DateTime>.Empty);
    
    private readonly ImmutableList<DateTime> _values;
    
    public ExceptionDates(IEnumerable<DateTime> values)
    {
        _values = values.ToImmutableList();
    }
    
    public ExceptionDates Add(DateTime value)
        => new ExceptionDates(_values.Add(value));
    
    
    public ExceptionDates AddRange(IEnumerable<DateTime> values)
        => new ExceptionDates(_values.AddRange(values));
    
    public IEnumerator<DateTime> GetEnumerator()
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
        
        result = new ExceptionDates(values);
        return true;
    }
}
