using System.Text.Json.Serialization;

namespace Calendar.Domain.Model;

public sealed class Session : User
{
    [JsonPropertyName("accessToken")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenReading)]
    public string? Token { get; init; }
    
    public string? Provider { get; init; } 
    
    public string? ProviderKey { get; init; }
    
    public bool RememberMe { get; init; }
    
    public bool LockoutOnFailure { get; init; }
}
