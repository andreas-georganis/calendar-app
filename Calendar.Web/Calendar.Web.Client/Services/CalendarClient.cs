using System.Net.Http.Json;
using Calendar.Contracts;

namespace Calendar.Web.Client.Services;

public interface ICalendarClient
{
    Task<Calendar?> GetCalendar(Guid calendarId, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Calendar>> GetCalendars(CancellationToken cancellationToken = default);
    
    Task NewCalendar(Calendar calendar, CancellationToken cancellationToken = default);
    
    Task EditCalendar(Guid id, Calendar calendar, CancellationToken cancellationToken = default);
    
    Task DeleteCalendar(Guid id, CancellationToken cancellationToken = default);
}

public class CalendarClient(HttpClient http) : ICalendarClient
{
    public async Task<Calendar?> GetCalendar(Guid calendarId, CancellationToken cancellationToken = default)
    {
        var calendar = await http.GetFromJsonAsync<Calendar>($"calendars/{calendarId}", cancellationToken);
        
        return calendar;
    }
    
    public async Task<IEnumerable<Calendar>> GetCalendars(CancellationToken cancellationToken = default)
    {
        var calendars = await http.GetFromJsonAsync<IReadOnlyList<Calendar>>("calendars", cancellationToken);
        
        return calendars ?? [];
    }

    public async Task NewCalendar(Calendar calendar, CancellationToken cancellationToken = default)
    {
        var response = await http.PostAsJsonAsync("calendars", calendar, cancellationToken);
        
        response.EnsureSuccessStatusCode();
    }

    public async Task EditCalendar(Guid id, Calendar calendar, CancellationToken cancellationToken = default)
    {
        var response = await http.PutAsJsonAsync($"calendars/{id}", calendar, cancellationToken);
        
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteCalendar(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await http.DeleteAsync($"calendars/{id}", cancellationToken);
        
        response.EnsureSuccessStatusCode();
    }
}
