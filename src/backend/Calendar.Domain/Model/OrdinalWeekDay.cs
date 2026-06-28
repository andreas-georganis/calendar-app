
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using NodaTime;

namespace Calendar.Domain.Model;

public readonly partial record struct OrdinalWeekDay : IParsable<OrdinalWeekDay>
{
    [GeneratedRegex(@"^(?<ordinal>[+-]?\d{1,2})?(?<day>MO|TU|WE|TH|FR|SA|SU)$", RegexOptions.Compiled | RegexOptions.IgnoreCase, 2000)]
    static partial Regex OrdinalWeekDayRegex { get; }
    
    public OrdinalWeekDay(IsoDayOfWeek value, YearWeek? ordinal = null)
    {
        Value = value;
        Ordinal = ordinal;
    }
    
    public IsoDayOfWeek Value { get; }
    
    public YearWeek? Ordinal { get; }


    public static OrdinalWeekDay Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var result) ? result : throw new FormatException();

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out OrdinalWeekDay result)
    {
        if (s is null or {Length: < 2})
        {
            result = default;
            return false;
        }
        
        var match = OrdinalWeekDayRegex.Match(s);
        
        if (!match.Success)
        {
            result = default;
            return false;
        }

        var day = match.Groups["day"].Value;
        var ordinal = match.Groups["ordinal"].Value;

        try
        {
            var weekDay = ParseDay(day);
            YearWeek? yearWeek = YearWeek.TryParse(ordinal, provider, out var value) ? value : null;
            
            result = new OrdinalWeekDay(weekDay, yearWeek);
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            result = default;
            return false;
        }
    }
    
    public static IsoDayOfWeek ParseDay(string value) =>
        value switch
        {
            "MO" => IsoDayOfWeek.Monday,
            "TU" => IsoDayOfWeek.Tuesday,
            "WE" => IsoDayOfWeek.Wednesday,
            "TH" => IsoDayOfWeek.Thursday,
            "FR" => IsoDayOfWeek.Friday,
            "SA" => IsoDayOfWeek.Saturday,
            "SU" => IsoDayOfWeek.Sunday,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "{value} is not a valid day of week")
        };
}
