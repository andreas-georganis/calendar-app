using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Calendar.Contracts;

public enum UserLookupBy
{
    Id,
    Email,
    Username,
    External
}

public sealed class UserLookupQuery
{
    /// <summary>
    /// The lookup method.
    /// </summary>
    public UserLookupBy By { get; init; }

    /// <summary>
    /// The external provider name (e.g., "Google", "Facebook") if looking up by external login.
    /// </summary>
    public string? Provider { get; init; }
}

public interface IUser
{
    Guid Id { get; }
    
    string? Email { get; }
    
    string? Username { get; }
    
    string? Password { get; }
}

public class User : IUser
{
    public Guid Id { get; init; }
    
    [EmailAddress]
    public string? Email { get; init; }

    [MinLength(3), MaxLength(20), RegularExpression("^[a-zA-Z0-9._-]+$", ErrorMessage = "Only letters, numbers, dot, underscore, dash.")]
    public string? Username { get; init; }
    
    [Required] 
    [StringLength(32, MinimumLength = 6, ErrorMessage = "The password must be between 6 and 32 characters long.")]
    [RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z])(?=.*[^a-zA-Z\\d]).*$", 
        MatchTimeoutInMilliseconds = 1000,
        ErrorMessage = "The password must contain a lower-case letter, an upper-case letter, a digit and a special character.")]
    public string? Password { get; init; }
}

public class Session : User
{
    [property: JsonPropertyName("accessToken")]
    public string? Token { get; init; }
    
    public string? Provider { get; init; } 
    
    public string? ProviderKey { get; init; }
    
    public bool RememberMe { get; init; }
    
    public bool LockoutOnFailure { get; init; }
}
