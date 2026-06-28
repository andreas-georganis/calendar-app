using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Calendar.Domain.Model;

public readonly record struct Period : IParsable<Period>
{
    public Period(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }

    public Period(DateTime start, Duration duration)
    {
        Start = start;
        Duration = duration;
    }
    
    public DateTime Start { get; init; }
        
    public DateTime? End { get; init; }
    
    public Duration? Duration { get; init; }

    public static Period Parse(string s, IFormatProvider? provider) 
        => TryParse(s, provider, out var result) ? result : throw new FormatException();

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Period result)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            result = default;
            return false;
        }

        var segments = s.Split("/", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        
        if (segments.Length != 2)
        {
            result = default;
            return false;
        }

        if (!DateTime.TryParse(segments[0], provider, out var dateTime))
        {
            result = default;
            return false;
        }
        
        if (!DateTimeOrDuration.TryParse(segments[1], provider, out var dateTimeOrDuration))
        {
            result = default;
            return false;
        }

        if (dateTimeOrDuration.TryGetValue(out DateTime? dt))
        {
            result = new Period(dateTime, dt.Value);
            return true;
        }

        if (dateTimeOrDuration.TryGetValue(out Duration? duration))
        {
            result = new Period(dateTime, duration.Value);
            return true;
        }
        
        
        result = default;
        return false;
    }
}
