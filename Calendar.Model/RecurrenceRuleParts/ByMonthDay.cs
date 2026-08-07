using System.Collections.Immutable;

namespace Calendar.API.Model.RecurrenceRuleParts;

public sealed class ByMonthDay
{
    public IEnumerable<MonthDay> Value { get; }
    
    public ByMonthDay(IEnumerable<MonthDay> value)
    {
        Value = value.ToImmutableHashSet();
    }
}
