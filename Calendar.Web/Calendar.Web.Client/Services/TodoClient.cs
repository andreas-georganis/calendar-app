using System.Net.Http.Json;
using Calendar.Contracts;
using DateTime = Calendar.Contracts.DateTime;

namespace Calendar.Web.Client.Services;

public interface ITodoClient
{
    
}

public class TodoClient(HttpClient http)
{
    public async Task<IEnumerable<Todo>> GetEntries(IReadOnlyCollection<Guid> calendarIds, DateTime? from, DateTime? to, CancellationToken cancellationToken = default)
    {
        var ids = string.Join("&", calendarIds.Select(id => $"calendarIds={id}"));
        var url = $"entries?{ids}&from={from}&to={to}";
        
        return await http.GetFromJsonAsync<IReadOnlyList<Todo>>(url, cancellationToken) ?? [];
    }

    public async Task<Entry?> NewEntry(Guid calendarId, Entry entry, CancellationToken cancellationToken = default)
    {
        var response = await http.PostAsJsonAsync($"entries", entry, cancellationToken);
        
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        
        return await response.Content.ReadFromJsonAsync<Entry>(cancellationToken);
    }

    public async Task EditEntry(Guid id, Entry entry, CancellationToken cancellationToken = default)
    {
        var response = await http.PutAsJsonAsync($"entries/{id}", entry, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteEntry(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await http.DeleteAsync($"entries/{id}", cancellationToken);
        response.EnsureSuccessStatusCode();
    }
    
    public async Task Complete(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await http.PutAsync($"entries/{id}/complete", null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
