using System.Collections.Immutable;

namespace Calendar.API.Model.RecurrenceRuleParts;

public sealed class ByYearDay
{
    public IEnumerable<YearDay> Value { get; }
    
    public ByYearDay(IEnumerable<YearDay> value)
    {
        Value = value.ToImmutableHashSet();
    }
}
