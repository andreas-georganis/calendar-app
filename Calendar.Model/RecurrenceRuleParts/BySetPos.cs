using System.Collections.Immutable;

namespace Calendar.API.Model.RecurrenceRuleParts;

public sealed class BySetPos
{
    public IEnumerable<SetPos> Value { get; }

    public BySetPos(IEnumerable<SetPos> value)
    {
        Value = value.ToImmutableHashSet();
    }
}
