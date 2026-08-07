using System.Net.Http.Json;
using System.Text;
using Calendar.Contracts;

namespace Calendar.Web.Client.Services;

public interface IIdentityResult;

public readonly struct IdentityResult : IIdentityResult
{
    private bool Success { get; init; }
    
    public static implicit operator IdentityResult(bool success) => new() { Success = success };
    
    public static implicit operator bool(IdentityResult result) => result.Success;
}

public interface IIdentityClient<TIdentityResult>
where TIdentityResult : IIdentityResult
{
    Task<TIdentityResult> Register(User user, CancellationToken cancellationToken = default);
    
    Task<TIdentityResult> Login(User user, CancellationToken cancellationToken = default);
    
    Task<bool> Logout();
}

public class IdentityClient(HttpClient http): IIdentityClient<IdentityResult>
{
    public async Task<IdentityResult> Register(User user, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
        {
            return false;
        }
        
        var response = await http.PostAsJsonAsync(
            "identity/register",
            user, cancellationToken);
        
        return response.IsSuccessStatusCode;
    }

    public async Task<IdentityResult> Login(User user, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
        {
            return false;
        }
        
        var response = await http.PostAsJsonAsync(
            "identity/login",
            user, cancellationToken);
        
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> Logout()
    {
        const string empty = "{}";
        var emptyContent = new StringContent(empty, Encoding.UTF8, "application/json");
        var response = await http.PostAsync("identity/logout", emptyContent);
        
        return response.IsSuccessStatusCode;
    }
}
