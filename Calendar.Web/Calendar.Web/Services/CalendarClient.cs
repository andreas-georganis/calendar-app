using CalendarApp.Contracts;
using Calendar.Web.Client.Services;

namespace Calendar.Web.Services;

public class CalendarClient(HttpClient http) : ICalendarClient
{
    public async Task<Calendar?> GetCalendar(Guid calendarId, CancellationToken cancellationToken = default)
    {
        var calendar = await http.GetFromJsonAsync<Calendar>($"api/calendars/{calendarId}", cancellationToken);
        
        return calendar;
    }
    
    public async Task<IEnumerable<Calendar>> GetCalendars(CancellationToken cancellationToken = default)
    {
        var calendars = await http.GetFromJsonAsync<IReadOnlyList<Calendar>>("api/calendars", cancellationToken);
        
        return calendars ?? [];
    }

    public async Task NewCalendar(Calendar calendar, CancellationToken cancellationToken = default)
    {
        var response = await http.PostAsJsonAsync("api/calendars", calendar, cancellationToken);
        
        response.EnsureSuccessStatusCode();
    }

    public async Task EditCalendar(Guid id, Calendar calendar, CancellationToken cancellationToken = default)
    {
        var response = await http.PutAsJsonAsync($"api/calendars/{id}", calendar, cancellationToken);
        
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteCalendar(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await http.DeleteAsync($"api/calendars/{id}", cancellationToken);
        
        response.EnsureSuccessStatusCode();
    }
}
