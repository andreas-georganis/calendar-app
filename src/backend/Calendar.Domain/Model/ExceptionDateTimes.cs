using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Calendar.Domain.Model;

public sealed class ExceptionDateTimes : IEnumerable<DateTime>, IParsable<ExceptionDateTimes>
{
    static ExceptionDateTimes Empty => new ExceptionDateTimes(ImmutableList<DateTime>.Empty);
    
    private readonly ImmutableList<DateTime> _values;
    
    public ExceptionDateTimes(IEnumerable<DateTime> values)
    {
        _values = values.ToImmutableList();
    }
    
    public ExceptionDateTimes Add(DateTime value)
        => new ExceptionDateTimes(_values.Add(value));
    
    
    public ExceptionDateTimes AddRange(IEnumerable<DateTime> values)
        => new ExceptionDateTimes(_values.AddRange(values));
    
    public IEnumerator<DateTime> GetEnumerator()
    {
        return _values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public static ExceptionDateTimes Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var result) ? result : throw new FormatException();

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out ExceptionDateTimes result)
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
        
        result = new ExceptionDateTimes(values);
        return true;
    }
}
