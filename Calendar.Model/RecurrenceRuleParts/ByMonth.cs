using System.Collections.Immutable;

namespace Calendar.API.Model.RecurrenceRuleParts;

public sealed class ByMonth
{
    public IEnumerable<Month> Value { get; }

    public ByMonth(IEnumerable<Month> value)
    {
        Value = value.ToImmutableHashSet();
    }
}
