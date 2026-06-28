using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace Calendar.Domain.Model;

public sealed class User : IdentityUser<Guid>
{
    public User()
    {
        
    }
    
    public User(string username, string email)
    {
        UserName = string.IsNullOrWhiteSpace(username) ? email : username;
        Email = email;
    }

    public string? Username => UserName;
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWriting)]
    public string? Password => base.PasswordHash;
    
}
