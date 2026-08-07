using System.Collections.Immutable;

namespace Calendar.API.Model.RecurrenceRuleParts;

public sealed class BySecond 
{
    public IEnumerable<Second> Value { get; }

    public BySecond(IEnumerable<Second> values)
    {
        Value = values.ToImmutableHashSet();
    }
}
