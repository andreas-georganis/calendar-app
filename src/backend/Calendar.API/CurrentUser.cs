using System.Security.Claims;
using Calendar.Domain.Model;
using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Calendar.API;

public class CurrentUser
{
    public ClaimsPrincipal Principal { get; }
    
    public OptionAsync<User> User { get; }

    public CurrentUser(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
    {
        var principal = httpContextAccessor.HttpContext?.User?? 
                        new ClaimsPrincipal();
        
        Principal = principal;
        
        User = OptionAsync<ClaimsPrincipal>.Some(principal)
            .BindAsync<User>(async p =>
            {
                if (!p.Identity?.IsAuthenticated ?? true)
                {
                    return OptionAsync<User>.None;
                }
                
                var userId = p.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId is null)
                {
                    return OptionAsync<User>.None;
                }

                var user = await userManager.FindByIdAsync(userId);
                return user is null 
                    ? OptionAsync<User>.None 
                    : OptionAsync<User>.Some(user);
            });
        
        Id = Guid.TryParse(Principal.FindFirstValue(ClaimTypes.NameIdentifier), out var id)? id: Guid.Empty;
    }
    
    public Guid Id
    {
        get;
    }
    
}

class CurrentUserRequirement : IAuthorizationRequirement;

class CurrentUserHandler : AuthorizationHandler<CurrentUserRequirement, CurrentUser>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CurrentUserRequirement requirement,
        CurrentUser resource)
    {
        if (await resource.User.IsSome)
        {
            context.Succeed(requirement);
        }

        return;
    }
}

public static class CurrentUserExtensions
{
    public static AuthorizationBuilder AddCurrentUser(this AuthorizationBuilder builder)
    {
        builder.Services.AddScoped<CurrentUser>();
        return builder;
    } 
    
    public static AuthorizationBuilder AddCurrentUserHandler(this AuthorizationBuilder builder)
    {
        builder.Services.AddScoped<IAuthorizationHandler, CurrentUserHandler>();
        return builder;
    }
    
    public static AuthorizationPolicyBuilder RequireCurrentUser(this AuthorizationPolicyBuilder builder)
    {
        return builder.RequireAuthenticatedUser()
            .AddRequirements(new CurrentUserRequirement());
    }
}
