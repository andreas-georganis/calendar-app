using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace Calendar.Domain.Model;

public class User : IdentityUser<Guid>
{
    public User()
    {
        
    }
    
    [JsonConstructor]
    public User(string username, string email)
    {
        UserName = string.IsNullOrWhiteSpace(username) ? email : username;
        Email = email;
    }
    
    [NotMapped]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWriting)]
    public string? Password { get; init; }
    
}
