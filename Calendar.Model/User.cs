using Calendar.Contracts;
using Microsoft.AspNetCore.Identity;

namespace Calendar.API.Model;

public class User : IdentityUser<Guid>, IUser
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
    public string? Password => base.PasswordHash;
}
