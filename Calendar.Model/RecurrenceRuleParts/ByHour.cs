
using System.Collections.Immutable;

namespace Calendar.API.Model.RecurrenceRuleParts;

public class ByHour
{
    public IEnumerable<Hour> Value { get; }

    public ByHour(IEnumerable<Hour> values)
    {
        Value = values.ToImmutableHashSet();
    }
}
