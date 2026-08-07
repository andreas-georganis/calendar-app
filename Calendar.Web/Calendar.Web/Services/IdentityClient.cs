using CalendarApp.Contracts;
using Calendar.Web.Client.Services;

namespace Calendar.Web.Services;

public readonly struct IdentityResult : IIdentityResult, IEquatable<IdentityResult>
{
    public static IdentityResult Failed => new IdentityResult(){Success = false};
    
    public static IdentityResult Suceeded(string token) => new IdentityResult(){Success = true, Token = token};
    
    public bool Success { get; init; }

    public string Token { get; init; }

    public bool Equals(IdentityResult other)
    {
        return Success == other.Success && Token == other.Token;
    }

    public override bool Equals(object? obj)
    {
        return obj is IdentityResult other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Success, Token);
    }

    public static bool operator ==(IdentityResult left, IdentityResult right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(IdentityResult left, IdentityResult right)
    {
        return !left.Equals(right);
    }
}


public class IdentityClient: IIdentityClient<IdentityResult>
{
    private readonly IUserClient _userClient;
    private readonly ISessionClient _sessionClient;

    public IdentityClient(IUserClient userClient, ISessionClient sessionClient)
    {
        _userClient = userClient;
        _sessionClient = sessionClient;
    }
    
    public async Task<IdentityResult> Register(User user, CancellationToken cancellationToken = default)
    {
        var userResource = await _userClient.NewUser(user, cancellationToken);

        if (userResource is null)
        {
            return IdentityResult.Failed;
        }

        return await Login(user, cancellationToken);
    }

    public async Task<IdentityResult> Login(User user, CancellationToken cancellationToken = default)
    {
        var session= await _sessionClient.NewSession(user, cancellationToken);
        
        if (session is null)
        {
            return IdentityResult.Failed;
        }
        
        return IdentityResult.Suceeded(session.Token);
    }

    public Task<bool> Logout()
    {
        //no-op since we are using cookie authentication and the cookie will be cleared on the client side
        return Task.FromResult(true);
    }
}
