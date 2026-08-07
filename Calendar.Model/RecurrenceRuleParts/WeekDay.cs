
using NodaTime;

namespace Calendar.API.Model.RecurrenceRuleParts;

public record WeekDay
{
    private WeekDay(){}
    public WeekDay(IsoDayOfWeek value, Ordinal? ordinal = null)
    {
        Value = value;
        Ordinal = ordinal;
    }
    
    public IsoDayOfWeek Value { get; }
    
    public Ordinal? Ordinal { get; }

    
}
