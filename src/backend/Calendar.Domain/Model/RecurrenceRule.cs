using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Calendar.Domain.Model;

public enum Frequency
{
    Secondly,
    Minutely,
    Hourly,
    Daily,
    Weekly,
    Monthly,
    Yearly
}


public class RecurrenceRule
{
    protected RecurrenceRule() { }

    [JsonConstructor]
    public RecurrenceRule(
        Frequency frequency, 
        Interval? interval = null, 
        UntilOrCount? untilOrCount = null,
        BySecond? bySecond = null,
        ByMinute? byMinute = null,
        ByHour? byHour = null,
        ByDay? byDay = null,
        ByWeekNo? byWeek = null,
        ByMonthDay? byMonthDay = null,
        ByMonth? byMonth = null,
        ByYearDay? byYearDay = null,
        BySetPos? bySetPos = null)
    {
        if (frequency == Frequency.Weekly && byMonthDay != null)
        {
            throw new InvalidOperationException("BYMONTHDAY not allowed with WEEKLY");
        }
        
        if (byYearDay != null && frequency != Frequency.Yearly)
        {
            throw new InvalidOperationException("BYYEARDAY only allowed with YEARLY");
        }
        
        if (byDay?.Values.Any(d => d.Ordinal is not null) == true &&
            frequency != Frequency.Monthly &&
            frequency != Frequency.Yearly)
        {
            throw new InvalidOperationException("Ordinal BYDAY only valid for MONTHLY or YEARLY");
        }
        
        if (frequency == Frequency.Yearly &&
            byWeek != null &&
            byDay?.Values.Any(d => d.Ordinal is not null) == true)
        {
            throw new InvalidOperationException(
                "Numeric BYDAY not allowed with YEARLY when BYWEEKNO is present");
        }
        
        if (bySetPos != null && NoOtherByParts())
        {
            throw new InvalidOperationException("BYSETPOS requires another BY*");
        }
        
        Frequency = frequency;
        Interval = interval;
        UntilOrCount = untilOrCount;
        BySecond = bySecond;
        ByMinute = byMinute;
        ByHour = byHour;
        ByDay = byDay;
        ByWeek = byWeek;
        ByMonthDay = byMonthDay;
        ByMonth = byMonth;
        ByYearDay = byYearDay;
        BySetPos = bySetPos;
        return;

        bool NoOtherByParts()
            => bySecond == null && byMinute == null && byHour == null && byDay == null && byMonth == null && byMonthDay == null && byYearDay == null;
    }

    

    public Frequency Frequency { get; }

    public Interval? Interval
    {
        get;
        init
        {
            field = value ?? Domain.Model.Interval.One();
        }
    } 
    
    public UntilOrCount? UntilOrCount { get; set; }
    
    public BySecond? BySecond { get;  }
    public ByMinute? ByMinute { get; }
    public ByHour? ByHour { get; }
    public ByDay? ByDay { get; }
    public ByWeekNo? ByWeek { get; }
    public ByMonthDay? ByMonthDay { get; }
    public ByMonth? ByMonth { get; }
    public ByYearDay? ByYearDay { get;  }
    public BySetPos? BySetPos { get; }
    
    
    
    public DayOfWeek WeekStart { get; } = DayOfWeek.Monday;
}



public sealed class RecurrenceRuleTemplate : RecurrenceRule
{
    [Required]
    public required string Name { get; init; }
}
