using System.Collections;
using System.Collections.Immutable;

namespace Calendar.API.Model;

public sealed class RecurrenceDates : IEnumerable<DateTime>
{
    private readonly ImmutableList<DateTime> _values;

    public RecurrenceDates(IEnumerable<DateTime> values)
    {
        _values = values.ToImmutableList();
    }
    
    public RecurrenceDates Add(DateTime value)
        => new RecurrenceDates(_values.Add(value));
    
    public RecurrenceDates AddRange(IEnumerable<DateTime> values)
        => new RecurrenceDates(_values.AddRange(values));
    
    
    public IEnumerator<DateTime> GetEnumerator()
    {
        return _values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
