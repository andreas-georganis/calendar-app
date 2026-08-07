using CalendarApp.Contracts;
using Calendar.Web.Client.Services;

namespace Calendar.Web.Services;

public class EntryClient(HttpClient http) : IEntryClient
{
    public Task<IEnumerable<Entry>> GetEntries(IReadOnlyCollection<Guid> calendarId, DateTime? from,
        DateTime? to,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

   
}
