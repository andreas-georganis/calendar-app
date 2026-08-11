using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using NodaTime;
using NodaTime.Text;

namespace Calendar.Domain.Model;

[JsonConverter(typeof(ParsableJsonConverter<DateTime>))]
public readonly partial record struct DateTime
    : IParsable<DateTime>, IComparable<DateTime>
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

    private readonly Instant? _utc;
    private readonly LocalDateTime? _floating;
    private readonly ZonedDateTime? _zoned;
    private readonly LocalDate? _date;

    internal DateTime(LocalDate date)
    {
        _date = date;
        Value = date.AtMidnight().InUtc().ToInstant();
    }

    internal DateTime(LocalDate date, LocalTime time)
        : this(date.At(time))
    {
    }

    internal DateTime(LocalDate date, LocalTime time, DateTimeZone zone)
        : this(date.At(time), zone)
    {

    }

    internal DateTime(LocalDateTime floating)
    {
        _floating = floating;
        Value = floating.InUtc().ToInstant();
    }

    private DateTime(LocalDateTime dateTime, DateTimeZone zone)
        : this(dateTime.InZoneLeniently(zone))
    {

    }

    private DateTime(Instant utc)
    {
        _utc = utc;
        Value = utc;
    }

    private DateTime(ZonedDateTime zoned)
    {
        _zoned = zoned;
        Value = zoned.ToInstant();
    }

    public Instant Value { get; }

    public LocalDate Date
        => _date ?? _zoned?.Date ?? _floating?.Date ?? _utc?.InUtc().Date ?? throw new InvalidOperationException("No valid date representation available.");

    public LocalTime? Time
        => _floating?.TimeOfDay ?? _zoned?.TimeOfDay ?? _utc?.InUtc().TimeOfDay;

    public DateTimeZone? Zone
        => _zoned?.Zone;

    public static DateTime operator +(DateTime dateTime, Duration duration)
        => new DateTime(dateTime.Value + duration.GetTime());

    public static DateTime operator -(DateTime dateTime, Duration duration)
        => new DateTime(dateTime.Value - duration.GetTime());

    public static DateTime Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var result)
            ? result
            : throw new FormatException($"Cannot parse '{s}' as a DateTime.");

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out DateTime result)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            result = default;
            return false;
        }

        if (LocalDateRegex.IsMatch(s)
            && LocalDatePattern.Iso.Parse(s).TryGetValue(default, out var date))
        {
            result = new DateTime(date);
            return true;
        }

        if (InstantRegex.IsMatch(s)
            && InstantPattern.ExtendedIso.Parse(s).TryGetValue(default, out var utc))
        {
            result = new DateTime(utc);
            return true;
        }

        if (LocalDateTimeRegex.IsMatch(s)
            && LocalDateTimePattern.ExtendedIso.Parse(s).TryGetValue(default, out var floating))
        {
            result = new DateTime(floating);
            return true;
        }

        if (ZonedDateTimeRegex.IsMatch(s)
            && ZonedDateTimePattern.ExtendedFormatOnlyIso.Parse(s).TryGetValue(default, out var zoned))
        {
            result = new DateTime(zoned);
            return true;
        }

        result = default;
        return false;
    }

    public override string ToString()
    {
        FormattableString formattable =
            (_utc, _floating, _zoned, _date) switch
            {
                ({ } utc, _, _, _) => $"{utc}",
                (_, { } floating, _, _) => $"{floating}",
                (_, _, { } zoned, _) => $"{zoned}",
                (_, _, _, { } date) => $"{date}",
                _ => throw new FormatException()
            };

        return formattable.ToString();
    }

    public int CompareTo(DateTime other)
    {
        return Value.CompareTo(other.Value);
    }

    public static bool operator >(DateTime left, DateTime right)
        => left.CompareTo(right) > 0;
    
    public static bool operator <(DateTime left, DateTime right)
        => left.CompareTo(right) < 0;

    public static bool operator >=(DateTime left, DateTime right)
        => left.CompareTo(right) >= 0;

    public static bool operator <=(DateTime left, DateTime right)
        => left.CompareTo(right) <= 0;
}
