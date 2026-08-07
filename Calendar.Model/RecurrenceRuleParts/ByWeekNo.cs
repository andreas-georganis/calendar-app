using System.Collections.Immutable;

namespace Calendar.API.Model.RecurrenceRuleParts;

public class ByWeekNo
{
    public IEnumerable<WeekNo> Value { get; }

    public ByWeekNo(IEnumerable<WeekNo> value)
    {
        Value = value.ToImmutableHashSet();
    }
}
