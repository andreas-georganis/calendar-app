using System.Collections.Immutable;

namespace Calendar.API.Model.RecurrenceRuleParts;

public sealed class ByDay
{
    public IEnumerable<WeekDay> Values { get; }

    public ByDay(IEnumerable<WeekDay> values)
    {
        Values = values.ToImmutableHashSet();
    }
}
