using System.Collections.Immutable;

namespace Calendar.API.Model.RecurrenceRuleParts;

public class ByMinute
{
    public IEnumerable<Minute> Value { get; }

    public ByMinute(IEnumerable<Minute> values)
    {
        Value = values.ToImmutableHashSet();
    }
}
