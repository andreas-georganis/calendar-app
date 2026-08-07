using CalendarApp.Contracts;
using Microsoft.AspNetCore.WebUtilities;

namespace Calendar.Web.Services;

public interface IUserClient
{
    Task<User?> NewUser(User userData, CancellationToken cancellationToken = default);
    
    Task<User?> GetUser(string id, UserLookupQuery userLookupQuery, CancellationToken cancellationToken = default);
}

public class UserClient(HttpClient http): IUserClient
{
    public async Task<User?> NewUser(User userData, CancellationToken cancellationToken = default)
    {
        var response = await http.PostAsJsonAsync(
            "identity/users",
            userData, cancellationToken);
        
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        
        var user = await response.Content.ReadFromJsonAsync<User>(cancellationToken);
        
        return user;
    }

    public async Task<User?> GetUser(string id, UserLookupQuery userLookupQuery, CancellationToken cancellationToken = default)
    {
        var builder = new Dictionary<string, string?>()
        {
            { "by", userLookupQuery.By.ToString().ToLower() },
            { "provider", userLookupQuery.Provider}
        };

        var url = QueryHelpers.AddQueryString(
            $"identity/users/{id}",
            builder);
        
        var response = await http.GetAsync(url, cancellationToken);
        
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        
        var user = await response.Content.ReadFromJsonAsync<User>(cancellationToken);
        
        return user;
    }
}
