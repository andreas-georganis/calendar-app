using System.Collections;
using System.Collections.Immutable;

namespace Calendar.API.Model;

public class ExceptionDates : IEnumerable<System.DateTime>
{
    private readonly ImmutableList<System.DateTime> _values;
    
    public ExceptionDates(IEnumerable<System.DateTime> values)
    {
        _values = values.ToImmutableList();
    }
    
    public ExceptionDates Add(System.DateTime value)
        => new ExceptionDates(_values.Add(value));
    
    
    public ExceptionDates AddRange(IEnumerable<System.DateTime> values)
        => new ExceptionDates(_values.AddRange(values));
    
    public IEnumerator<System.DateTime> GetEnumerator()
    {
        return _values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
