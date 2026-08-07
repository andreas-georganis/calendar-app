using System.Net.Http.Json;
using Calendar.Contracts;

namespace Calendar.Web.Client.Services;

public interface IEntryClient
{
    Task<IEnumerable<Entry>> GetEntries(IReadOnlyCollection<Guid> calendarId, Calendar.Contracts.DateTime? from, Calendar.Contracts.DateTime? to, CancellationToken cancellationToken = default);
}

public class EntryClient(HttpClient http) : IEntryClient
{
    public async Task<IEnumerable<Entry>> GetEntries(IReadOnlyCollection<Guid> calendarIds, Calendar.Contracts.DateTime? from, Calendar.Contracts.DateTime? to, CancellationToken cancellationToken = default)
    {
        var ids = string.Join("&", calendarIds.Select(id => $"calendarIds={id}"));
        var url = $"entries?{ids}&from={from}&to={to}";
        
        return await http.GetFromJsonAsync<IReadOnlyList<Entry>>(url, cancellationToken) ?? [];
    }
}
