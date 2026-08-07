using CalendarApp.Contracts;

namespace Calendar.Web.Services;

public interface ISessionClient
{
    Task<Session?> NewSession(User user, CancellationToken cancellationToken = default);
}

public class SessionClient(HttpClient http): ISessionClient
{
    public async Task<Session?> NewSession(User user, CancellationToken cancellationToken = default)
    {
        var response = await http.PostAsJsonAsync(
            "identity/sessions",
            user, cancellationToken);
        
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        
        var session = await response.Content.ReadFromJsonAsync<Session>(cancellationToken);
        
        return session;
    }
}
