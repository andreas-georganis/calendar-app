using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Calendar.Domain.Model;

public readonly record struct Period : IParsable<Period>
{
    public Period(CalDateTime start, CalDateTime end)
    {
        Start = start;
        End = end;
    }

    public Period(CalDateTime start, Duration duration)
    {
        Start = start;
        Duration = duration;
    }
    
    public CalDateTime Start { get; } 
       
    public CalDateTime? End { get; }
    
    public Duration? Duration { get; }

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

        if (!CalDateTime.TryParse(segments[0], provider, out var start))
        {
            result = default;
            return false;
        }

        if (Model.Duration.IsValid(segments[1]))
        {
            result = new Period(start, Model.Duration.Parse(segments[1], provider));
            return true;
        }
        
        if (!CalDateTime.TryParse(segments[1], provider, out var end))
        {
            result = default;
            return false;
        }
        
        result = new Period(start, end);
        return true;
    }
}
