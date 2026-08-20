
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;


namespace Calendar.Domain.Model;

[JsonConverter(typeof(ParsableJsonConverter<DateTimeOrDuration>))]
public readonly record struct DateTimeOrDuration: IParsable<DateTimeOrDuration>
{
    private readonly CalDateTime? _dateTime;
    private readonly Duration? _duration;
    
    public DateTimeOrDuration(CalDateTime dateTime)
    {
        _dateTime = dateTime;
        _duration = null;
    }
    
    public DateTimeOrDuration(Duration duration)
    {
        _duration = duration;
        _dateTime = null;
    }
    
    public CalDateTime? DateTime => _dateTime;

    public Duration? Duration => _duration;

    public bool IsDateTime => _dateTime is not null;
    
    public bool TryGetValue([NotNullWhen(true)]out CalDateTime? dateTime)
    {
        dateTime = _dateTime;
        return dateTime is not null;
    }
    
    public bool TryGetValue([NotNullWhen(true)] out Duration? duration)
    {
        duration = _duration;
        return duration is not null;
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
        
        Model.CalDateTime.TryParse(s,provider, out var dateTime);

        if (dateTime != default)
        {
            result = new DateTimeOrDuration(dateTime);
            return true;
        }
        
        result = default;
        return false;
    }

    public override string? ToString()
    {
        return IsDateTime ? _dateTime!.ToString() : _duration!.ToString();
    }
}
