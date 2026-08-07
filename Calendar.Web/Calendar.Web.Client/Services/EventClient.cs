using System.Net.Http.Json;
using Calendar.Contracts;
using DateTime = Calendar.Contracts.DateTime;

namespace Calendar.Web.Client.Services;

public interface IEventClient
{
    
}

public class EventClient(HttpClient http)
{
    public async Task<IEnumerable<Event>> GetEvents(IReadOnlyCollection<Guid> calendarIds, DateTime? from, DateTime? to, CancellationToken cancellationToken = default)
    {
        var ids = string.Join("&", calendarIds.Select(id => $"calendarIds={id}"));
        var url = $"entries?{ids}&from={from}&to={to}";
        
        return await http.GetFromJsonAsync<IReadOnlyList<Event>>(url, cancellationToken) ?? [];
    }

    public async Task<Entry?> NewEvent(Guid calendarId, Entry entry, CancellationToken cancellationToken = default)
    {
        var response = await http.PostAsJsonAsync($"entries", entry, cancellationToken);
        
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        
        return await response.Content.ReadFromJsonAsync<Entry>(cancellationToken);
    }

    public async Task EditEvent(Guid id, Entry entry, CancellationToken cancellationToken = default)
    {
        var response = await http.PutAsJsonAsync($"entries/{id}", entry, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteEvent(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await http.DeleteAsync($"entries/{id}", cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
