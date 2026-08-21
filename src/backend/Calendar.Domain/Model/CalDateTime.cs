using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using NodaTime;
using NodaTime.Text;

namespace Calendar.Domain.Model;

[JsonConverter(typeof(ParsableJsonConverter<CalDateTime>))]
//public readonly partial record struct CalDateTime : IParsable<CalDateTime>, IComparable<CalDateTime>
public partial record CalDateTime: IParsable<CalDateTime>, IComparable<CalDateTime>
{
    private const RegexOptions Options = RegexOptions.Compiled | RegexOptions.IgnoreCase;

    [GeneratedRegex(@"^\d{4}-\d{2}-\d{2}$", Options, 2000)]
    static partial Regex LocalDateRegex { get; }

    [GeneratedRegex(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}$", Options, 2000)]
    static partial Regex LocalDateTimeRegex { get; }

    [GeneratedRegex(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}Z$", Options, 2000)]
    static partial Regex InstantRegex { get; }

    [GeneratedRegex(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\s+[A-Za-z_]+(?:/[A-Za-z_]+)+$", Options, 2000)]
    static partial Regex ZonedDateTimeRegex { get; }

    private readonly LocalDate? _date;
    private readonly LocalDateTime? _dateTime;
    private readonly DateTimeZone? _zone;
    private readonly Instant? _utc;
    

    public CalDateTime(LocalDate? date)
        : this(date: date, dateTime: null, zone: null, utc: null)
    {
    }

    public CalDateTime(LocalDate date, LocalTime time)
        : this(dateTime: date.At(TimeAdjusters.TruncateToSecond(time)))
    {
    }

    public CalDateTime(LocalDate date, LocalTime time, DateTimeZone zone)
        : this(dateTime: date.At(TimeAdjusters.TruncateToSecond(time)), zone: zone)
    {

    }

    public CalDateTime(LocalDateTime dateTime)
    : this(date: null, dateTime: dateTime, zone: null, utc: null)
    {
    }

    public CalDateTime(LocalDateTime dateTime, DateTimeZone zone)
        : this(date: null, dateTime: dateTime, zone: zone, utc: null)
    {
    }

    public CalDateTime(ZonedDateTime zoned)
        : this(date: null, dateTime: zoned.ToOffsetDateTime().LocalDateTime, zone: zoned.Zone, utc: null)
    {
        
    }

    public CalDateTime(Instant utc)
        : this(date: null, dateTime: null, zone: null, utc: utc)
    {
       
    }

    private CalDateTime(LocalDate? date, LocalDateTime? dateTime, DateTimeZone? zone, Instant? utc)
    {
        _date = date;
        _dateTime = dateTime;
        _utc = utc;
        
        _zone = zone;
        
        Value = utc ?? 
            dateTime?.InZoneLeniently(zone ?? DateTimeZone.Utc).ToInstant() ?? 
            date?.AtStartOfDayInZone(DateTimeZone.Utc).ToInstant() ??
            throw new ArgumentException("At least one of the parameters must be provided.");
    }

    public Instant Value { get; }

    public bool IsDateOnly => _date is not null;

    public bool IsFloating => _zone is null && _dateTime is not null;

    public LocalDate Date
        => _date ?? _dateTime?.Date ?? _utc?.InUtc().Date ?? throw new InvalidOperationException("No valid date representation available.");

    public LocalTime? Time
        => _dateTime?.TimeOfDay ?? _utc?.InUtc().TimeOfDay;

    public DateTimeZone? Zone 
        => _zone;

    public static CalDateTime operator +(CalDateTime dateTime, Duration duration)
        => new CalDateTime(dateTime.Value + duration.Value);

    public static CalDateTime operator -(CalDateTime dateTime, Duration duration)
        => new CalDateTime(dateTime.Value - duration.Value);

    public static CalDateTime Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var result)
            ? result
            : throw new FormatException($"Cannot parse '{s}' as a DateTime.");

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [NotNullWhen(true)] out CalDateTime? result)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            result = default;
            return false;
        }

        if (LocalDateRegex.IsMatch(s)
            && LocalDatePattern.Iso.Parse(s).TryGetValue(default, out var date))
        {
            result = new CalDateTime(date);
            return true;
        }

        if (InstantRegex.IsMatch(s)
            && InstantPattern.ExtendedIso.Parse(s).TryGetValue(default, out var utc))
        {
            result = new CalDateTime(utc);
            return true;
        }

        if (LocalDateTimeRegex.IsMatch(s)
            && LocalDateTimePattern.ExtendedIso.Parse(s).TryGetValue(default, out var floating))
        {
            result = new CalDateTime(floating);
            return true;
        }

        if (ZonedDateTimeRegex.IsMatch(s)
            && ZonedDateTimePattern.ExtendedFormatOnlyIso.Parse(s).TryGetValue(default, out var zoned))
        {
            result = new CalDateTime(zoned);
            return true;
        }

        result = default;
        return false;
    }

    public override string ToString()
    {
        FormattableString formattable =
            (_date, _dateTime, _utc) switch
            {
                ({ } date, _, _) => $"{date}",
                (_, { } dateTime, _) when _zone is null => $"{dateTime}",
                (_, _, { } utc) => $"{utc}",
                (_, { } dateTime, _) when _zone is { } zone => $"{dateTime} {zone.Id}",
                
                _ => throw new FormatException()
            };

        return formattable.ToString();
    }

    public int CompareTo(CalDateTime? other)
    {
        return other is null ? 1 : Value.CompareTo(other.Value);
    }

    public static bool operator >(CalDateTime left, CalDateTime right)
        => left.CompareTo(right) > 0;
    
    public static bool operator <(CalDateTime left, CalDateTime right)
        => left.CompareTo(right) < 0;

    public static bool operator >=(CalDateTime left, CalDateTime right)
        => left.CompareTo(right) >= 0;

    public static bool operator <=(CalDateTime left, CalDateTime right)
        => left.CompareTo(right) <= 0;
}
