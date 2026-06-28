using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using NodaTime;
using NodaTime.Text;

namespace Calendar.Domain.Model;

public readonly partial record struct DateTime
    : IParsable<DateTime>, IComparable<DateTime>
{
    private const RegexOptions Options = RegexOptions.Compiled | RegexOptions.IgnoreCase;

    [GeneratedRegex(@"^\d{4}-\d{2}-\d{2}$", Options, 2000)]
    static partial Regex LocalDateRegex { get; }

    [GeneratedRegex(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}$", RegexOptions.Compiled)]
    static partial Regex LocalDateTimeRegex { get; }

    [GeneratedRegex(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}Z$", RegexOptions.Compiled)]
    static partial Regex InstantRegex { get; }

    [GeneratedRegex(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\s+[A-Za-z_]+(?:/[A-Za-z_]+)+$", RegexOptions.Compiled)]
    static partial Regex ZonedDateTimeRegex { get; }

    private readonly Instant? _utc;
    private readonly LocalDateTime? _floating;
    private readonly ZonedDateTime? _zoned;
    private readonly LocalDate? _date;

    private DateTime(LocalDate date)
    {
        _date = date;
        Value = date.AtMidnight().InUtc().ToInstant();
    }

    private DateTime(LocalDate date, LocalTime time)
        : this(date.At(time))
    {
    }

    private DateTime(LocalDate date, LocalTime time, DateTimeZone zone)
        : this(date.At(time), zone)
    {

    }

    private DateTime(LocalDateTime floating)
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

        s = s.Trim();

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
