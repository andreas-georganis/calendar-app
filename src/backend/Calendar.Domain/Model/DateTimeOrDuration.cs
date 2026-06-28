
using System.Diagnostics.CodeAnalysis;

namespace Calendar.Domain.Model;

public readonly record struct DateTimeOrDuration: IParsable<DateTimeOrDuration>
{
    private readonly DateTime? _dateTime;
    private readonly Duration? _duration;
    
    public DateTimeOrDuration(DateTime dateTime)
    {
        _dateTime = dateTime;
    }
    
    public DateTimeOrDuration(Duration duration)
    {
        _duration = duration;
    }
    
    public DateTime? DateTime => _dateTime;

    public Duration? Duration => _duration;
    
    public bool TryGetValue([NotNullWhen(true)]out DateTime? dateTime)
    {
        dateTime = _dateTime;
        return dateTime.HasValue;
    }
    
    public bool TryGetValue([NotNullWhen(true)] out Duration? duration)
    {
        duration = _duration;
        return duration.HasValue;
    }

    public static DateTimeOrDuration Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var result) ? result: throw new FormatException();

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out DateTimeOrDuration result)
    {
        Model.Duration.TryParse(s, provider, out var duration);
        
        if (duration != default)
        {
            result = new DateTimeOrDuration(duration);
            return true;
        }
        
        Model.DateTime.TryParse(s,provider, out var dateTime);

        if (dateTime != default)
        {
            result = new DateTimeOrDuration(dateTime);
            return true;
        }
        
        result = default;
        return false;
    }
}
