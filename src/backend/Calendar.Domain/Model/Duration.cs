using System.Globalization;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using NodaTime;
using NodaTime.Text;

namespace Calendar.Domain.Model;

/// <summary>
/// ISO-8601-2 duration
/// </summary>
public readonly partial record struct Duration : IParsable<Duration>
{
    [GeneratedRegex(@"^(?<sign>[+-])?P(?:(?<weeks>[0-9]+)W|(?:(?<days>[0-9]+)D(?:T(?:(?<hours>[0-9]+)H(?:(?<minutes>[0-9]+)M(?:(?<seconds>[0-9]+)S)?)?|(?<minutes>[0-9]+)M(?:(?<seconds>[0-9]+)S)?|(?<seconds>[0-9]+)S))?)|T(?:(?<hours>[0-9]+)H(?:(?<minutes>[0-9]+)M(?:(?<seconds>[0-9]+)S)?)?|(?<minutes>[0-9]+)M(?:(?<seconds>[0-9]+)S)?|(?<seconds>[0-9]+)S))$",
        RegexOptions.Compiled)]
    private static partial Regex Rfc5545DurationRegex { get; }

    public static Duration Zero => Duration.Parse("P0D", null);

    public Duration(NodaTime.Period period)
        : this(period.Weeks, period.Days, (int)period.Hours, (int)period.Minutes, (int)period.Seconds)
    {}
    
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
    }

    public int? Weeks { get; private init; }
    public int? Days { get; private init; }
    public int? Hours { get; private init; }
    public int? Minutes { get; private init; }
    public int? Seconds { get; private init; }
    
    public NodaTime.Period GetDate()
    {
        var b = new PeriodBuilder
        {
            Weeks = Weeks ?? 0,
            Days = Days ?? 0,
            Hours = Hours ?? 0,
            Minutes = Minutes ?? 0,
            Seconds = Seconds ?? 0
        };

        return b.Build();
    }
    
    public NodaTime.Duration GetTime()
    {
        var b = new NodaTime.PeriodBuilder
        {
            Hours = Hours ?? 0,
            Minutes = Minutes ?? 0,
            Seconds = Seconds ?? 0,
        };

        return b.Build().ToDuration();
    }
    
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

        if (TryParseRfc5545(value, out result))
        {
            return true;
        }

        result = default;
        return false;
    }

    private static bool TryParseRfc5545(string value, out Duration result)
    {
        var match = Rfc5545DurationRegex.Match(value);

        if (!match.Success)
        {
            result = default;
            return false;
        }

        var sign = match.Groups["sign"].Value == "-" ? -1 : 1;

        if (!TryParseComponent(match, "weeks", sign, out var weeks)
            || !TryParseComponent(match, "days", sign, out var days)
            || !TryParseComponent(match, "hours", sign, out var hours)
            || !TryParseComponent(match, "minutes", sign, out var minutes)
            || !TryParseComponent(match, "seconds", sign, out var seconds))
        {
            result = default;
            return false;
        }

        result = new Duration
        {
            Weeks = weeks,
            Days = days,
            Hours = hours,
            Minutes = minutes,
            Seconds = seconds
        };
        return true;
    }

    private static bool TryParseComponent(Match match, string name, int sign, out int? result)
    {
        var group = match.Groups[name];

        if (!group.Success)
        {
            result = null;
            return true;
        }

        // -P2147483648D is valid
        if (!long.TryParse(group.Value, NumberStyles.None, CultureInfo.InvariantCulture, out var value))
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
       => new Duration(-duration.Weeks, -duration.Days, -duration.Hours, -duration.Minutes, -duration.Seconds);

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
