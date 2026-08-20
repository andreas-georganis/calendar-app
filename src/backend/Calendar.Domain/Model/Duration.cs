using System.Globalization;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using NodaTime;
using System.Text .Json.Serialization;

namespace Calendar.Domain.Model;

/// <summary>
/// ISO-8601-2 duration
/// </summary>
[JsonConverter(typeof(ParsableJsonConverter<Duration>))]
public readonly partial record struct Duration : IParsable<Duration>
{
    [GeneratedRegex(@"^(?<sign>[+-])?P(?=[0-9]+(?:W|D)|T[0-9]+[HMS])(?:(?<weeks>[0-9]+)W)?(?:(?<days>[0-9]+)D)?(?:T(?=[0-9]+[HMS])(?:(?<hours>[0-9]+)H)?(?:(?<minutes>[0-9]+)M)?(?:(?<seconds>[0-9]+)S)?)?$", RegexOptions.Compiled)]
    private static partial Regex DurationRegex { get; }

    public static Duration Zero => Duration.Parse("P0D", null);

    public static Duration OneDay => Duration.Parse("P1D", null);

    public static bool IsValid(string value)
        => DurationRegex.IsMatch(value);
    
    private readonly NodaTime.Duration _duration;

    public Duration(NodaTime.Period period)
        : this(period.ToDuration())
    {
       
    }

    public Duration(int? weeks = null, int? days = null, int? hours = null, int? minutes = null, int? seconds = null)
    {
        if (!SameSign(weeks, days, hours, minutes, seconds))
        {
            throw new ArgumentException("All values must have the same sign");
        }
        
        Weeks = weeks;
        Days = days;
        Hours = hours;
        Minutes = minutes;
        Seconds = seconds;

        var b = new PeriodBuilder
        {
            Weeks = Weeks ?? 0,
            Days = Days ?? 0,
            Hours = Hours ?? 0,
            Minutes = Minutes ?? 0,
            Seconds = Seconds ?? 0,
        };

        _duration = b.Build().ToDuration();
    }

    private Duration(NodaTime.Duration value)
    {
        _duration = value;
    }

    public int? Weeks { get; private init; }
    public int? Days { get; private init; }
    public int? Hours { get; private init; }
    public int? Minutes { get; private init; }
    public int? Seconds { get; private init; }

    public NodaTime.Duration Value => _duration;
    
    public static Duration Parse(string s, IFormatProvider? provider)
    {
        return TryParse(s, provider, out var result)
            ? result
            : throw new FormatException($"Invalid duration: '{s}'.");
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Duration result)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            result = default;
            return false;
        }

        var value = s.Trim();

        var match = DurationRegex.Match(value);

        if (!match.Success)
        {
            result = default;
            return false;
        }

        var sign = match.Groups["sign"].Value == "-" ? -1 : 1;

        if (!TryParseComponent(match, "weeks", sign, provider, out var weeks)
            || !TryParseComponent(match, "days", sign, provider, out var days)
            || !TryParseComponent(match, "hours", sign, provider, out var hours)
            || !TryParseComponent(match, "minutes", sign, provider, out var minutes)
            || !TryParseComponent(match, "seconds", sign, provider, out var seconds))
        {
            result = default;
            return false;
        }

        try
        {
            result = new Duration(weeks, days, hours, minutes, seconds);
            return true;
        }
        catch (ArgumentException)
        {
            result = default;
            return false;
        }
    }

    private static bool TryParseComponent(Match match, string name, int sign, IFormatProvider? provider, out int? result)
    {
        var group = match.Groups[name];

        if (!group.Success)
        {
            result = null;
            return true;
        }

        // -P2147483648D is valid
        if (!long.TryParse(group.Value, NumberStyles.None, provider, out var value))
        {
            result = null;
            return false;
        }

        var signedValue = value * sign;

        if (signedValue is > int.MaxValue or < int.MinValue)
        {
            result = null;
            return false;
        }

        result = (int)signedValue;
        return true;
    }
    
    public static Duration operator -(Duration duration)
       => new(-duration.Weeks, -duration.Days, -duration.Hours, -duration.Minutes, -duration.Seconds);

    private static bool SameSign(params int?[] values)
    {
        int? reference = null;
        foreach (var value in values)
        {
            if (!value.HasValue)
                continue;

            if (reference.HasValue)
            {
                if ((reference.Value ^ value.Value) < 0)
                    return false;
            }
            else
            {
                reference = value;
            }
        }
        
        return true;
    }
}
