using System.ComponentModel.DataAnnotations;

namespace Calendar.Contracts;

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



public sealed record WeekDay(NodaTime.IsoDayOfWeek Value, int? Ordinal);

public class RecurrenceRule
{
    public Frequency Frequency { get; init; }
    
    public DateTime? Until { get; init; }
    
    public int? Count { get; init; }
    
    public int? Interval { get; init; } = 1;
    public IReadOnlyCollection<int>? BySecond { get; init; }
    public IReadOnlyCollection<int>? ByMinute { get; init; }
    public IReadOnlyCollection<int>? ByHour { get; init; }
    public IReadOnlyCollection<WeekDay>? ByDay { get; init; }
    public IReadOnlyCollection<int>? ByWeek { get; init; }
    public IReadOnlyCollection<int>? ByMonthDay { get; init; }
    public IReadOnlyCollection<int>? ByMonth { get; init; }
    public IReadOnlyCollection<int>? ByYearDay { get; init; }
    public IReadOnlyCollection<int>? BySetPos { get; init; }
}



public sealed class RecurrenceRuleTemplate : RecurrenceRule
{
    [Required]
    public required string Name { get; init; }
}

public sealed class Recurrence
{
    public RecurrenceRule? Rule { get; init; }
    
    public IReadOnlyCollection<DateTime>? ExceptionDateTimes { get; init; }
    
    public required IReadOnlyCollection<DateTime>? DateTimes { get; init; }
    
    public required IReadOnlyCollection<Period>? Periods { get; init; }
}


