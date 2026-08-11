using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using NodaTime;

namespace Calendar.Domain.Model;



public class RecurrenceRule
{
    protected RecurrenceRule() { }

    [JsonConstructor]
    public RecurrenceRule(
        Frequency frequency,
        Interval? interval = null, 
        DateTime? until = null,
        Count? count = null,
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
        if (until != null && count != null)
        {
            throw new InvalidOperationException("UNTIL and COUNT cannot be set at the same time");
        }

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
        Interval = interval ?? Domain.Model.Interval.One();
        Until = until;
        Count = count;
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

    public Interval Interval { get; } 
    
    public DateTime? Until { get; }

    public Count? Count { get; }
    
    public BySecond? BySecond { get;  }
    public ByMinute? ByMinute { get; }
    public ByHour? ByHour { get; }
    public ByDay? ByDay { get; }
    public ByWeekNo? ByWeek { get; }
    public ByMonthDay? ByMonthDay { get; }
    public ByMonth? ByMonth { get; }
    public ByYearDay? ByYearDay { get;  }
    public BySetPos? BySetPos { get; }
    
    public IsoDayOfWeek WeekStart { get; } = IsoDayOfWeek.Monday;
}



public sealed class RecurrenceRuleTemplate : RecurrenceRule
{
    [Required]
    public required string Name { get; init; }
}
