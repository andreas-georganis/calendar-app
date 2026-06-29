using System.ComponentModel.DataAnnotations;
using Calendar.Domain.Model;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace Calendar.API.Endpoints;

public enum UserLookupBy
{
    Id,
    Email,
    Username,
    External
}

public static class IdentityApi
{
    private static readonly EmailAddressAttribute EmailAddressAttribute = new();
    
    public static RouteGroupBuilder MapIdentityApi(this RouteGroupBuilder routes)
    {
        var users = routes.MapGroup("/users")
            .WithTags("Users")
            .RequireAuthorization();

        var sessions = routes.MapGroup("/sessions")
            .WithTags("Sessions")
            .RequireAuthorization();
        
        // map identity routes
        //users.MapIdentityApi<User>();

        users.MapGet("{id}", async Task<Results<Ok<User>, BadRequest<string>, NotFound>> (
            string id,
            UserLookupBy by,
            string? provider,
            UserManager<User> userManager,
            IDataProtectionProvider dataProtectionProvider) =>
        {
            if (by == UserLookupBy.External && string.IsNullOrWhiteSpace(provider))
                return TypedResults.BadRequest("provider is required when by=external.");
            
            var user = by switch
            {
                UserLookupBy.Id => await userManager.FindByIdAsync(id),
                UserLookupBy.Email => await userManager.FindByEmailAsync(id),
                UserLookupBy.Username => await userManager.FindByNameAsync(id),
                UserLookupBy.External => string.IsNullOrWhiteSpace(provider)
                    ? null
                    : await FindByLoginAsync(userManager,dataProtectionProvider, provider, id),

                _ => null
            };

            if (user is null)
            {
                return TypedResults.NotFound();
            }
            

            return TypedResults.Ok(user);
            
            static Task<Domain.Model.User?> FindByLoginAsync(UserManager<Domain.Model.User> userManager, IDataProtectionProvider dataProtectionProvider, string provider, string protectedProviderKey)
            {
                var protector = dataProtectionProvider.CreateProtector(provider);

                var providerKey = protector.Unprotect(protectedProviderKey);

                return userManager.FindByLoginAsync(provider, providerKey);
            }
        });

        users.MapPost("/", async Task<Results<Created<User>, ValidationProblem, Conflict>> (
            User data, 
            UserManager<Domain.Model.User> userManager) =>
        {
            if (!userManager.SupportsUserEmail)
            {
                throw new NotSupportedException($"{nameof(IdentityApi)} requires a user store with email support.");
            }
            
            if (!EmailAddressAttribute.IsValid(data.Email))
            {
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    { nameof(data.Email), new[] { "Invalid email address format." } }
                });
            }

            Domain.Model.User user = new(data.UserName, data.Email);
            
            var result = await userManager.CreateAsync(user, data.Password);
            
            if (!result.Succeeded)
            {
                return TypedResults.ValidationProblem(result.Errors.ToDictionary(e => e.Code,
                    e => new[] { e.Description }));
            }
            
            return TypedResults.Created($"/users/{user.Id}", user);
        });

        users.MapGet("/me", async Task<Results<Ok<User>, UnauthorizedHttpResult>> (
            UserManager<Domain.Model.User> userManager,
            HttpContext httpContext) =>
        {
            var user = await userManager.GetUserAsync(httpContext.User);

            if (user is null)
            {
                return TypedResults.Unauthorized();
            }

            

            return TypedResults.Ok(user);
        });
        
        sessions.MapPost("", async (
            Session data,
            SignInManager<Domain.Model.User> signInManager,
            UserManager<Domain.Model.User> userManager) =>
        {
            signInManager.AuthenticationScheme = IdentityConstants.BearerScheme;
            
            // -----------------------
            // Local login: email/password
            // -----------------------
            if (string.IsNullOrWhiteSpace(data.Provider))
            {
                var result = await signInManager.PasswordSignInAsync(
                    data.UserName,
                    data.Password,
                    isPersistent: data.RememberMe,
                    lockoutOnFailure: data.LockoutOnFailure);

                if (!result.Succeeded)
                    return TypedResults.Unauthorized();

                // BearerToken handler already wrote AccessTokenResponse
                return TypedResults.Empty;
            }
            
            // -----------------------
            // External login: provider + providerKey (+ email for provisioning)
            // -----------------------
            if (string.IsNullOrWhiteSpace(data.ProviderKey))
                return TypedResults.BadRequest("providerKey is required when provider is provided.");
            
            var user = await userManager.FindByLoginAsync(data.Provider, data.ProviderKey);
            
            var identityResult = IdentityResult.Success;
            
            if (user is null)
            {
                if (string.IsNullOrWhiteSpace(data.Email))
                    return Results.BadRequest("email is required to provision/link an external user.");

                user = await userManager.FindByEmailAsync(data.Email);

                if (user is null)
                {
                    user = new Domain.Model.User(data.UserName, data.Email);

                    identityResult = await userManager.CreateAsync(user);
                }

                if (identityResult.Succeeded)
                {
                    identityResult = await userManager.AddLoginAsync(user, new UserLoginInfo(
                        data.Provider,
                        data.ProviderKey,
                        data.Provider));
                }
            }

            if (identityResult.Succeeded)
            {
                var principal = await signInManager.CreateUserPrincipalAsync(user);
                return TypedResults.SignIn(principal, authenticationScheme: IdentityConstants.BearerScheme);
            }
            
            return TypedResults.BadRequest(identityResult.Errors);
        });

        return routes;
    }
}
