
using Ical.Net;

namespace Calendar.API.Model;

public interface IOccurrenceCalculator
{
    IEnumerable<Entry> For(Entry entry, DateTime? start, DateTime? end);
}

public class OccurrenceCalculator : IOccurrenceCalculator
{
    public IEnumerable<Entry> For(Entry entry, DateTime? start, DateTime? end)
    {
        var icalModel = entry.ToIcal();
        
        var occurrences = icalModel.GetOccurrences(start?.ToIcal());

        if (end is not null)
        {
            occurrences = occurrences.TakeWhileBefore(end.ToIcal());
        }
        
        return occurrences.Select(o=> o.ToModel());
    }
}
