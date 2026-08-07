using System.Security.Claims;
using Calendar.Web.Services;
using CalendarApp.Contracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IdentityResult = Calendar.Web.Services.IdentityResult;
using IdentityResult = Microsoft.AspNetCore.Identity.IdentityResult;

namespace Calendar.Web.Apis;

internal static class IdentityApi
{
    private const string ProviderKey = "provider";
    
    internal static IEndpointRouteBuilder MapIdentityApi(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/identity").WithTags("Identity");

        group.MapPost("register", async Task<Results<SignInHttpResult, UnauthorizedHttpResult>> (
            User user, IdentityClient identityClient, CancellationToken cancellationToken = default) =>
        {
            var result = await identityClient.Register(user, cancellationToken);
            
            if (result == Services.IdentityResult.Failed)
            {
                return TypedResults.Unauthorized();
            }

            return SignIn(user, result.Token);
            
        }).AllowAnonymous();
        
        group.MapPost("login", async Task<Results<SignInHttpResult, UnauthorizedHttpResult>> (
            User user, IdentityClient identityClient, CancellationToken cancellationToken = default) =>
        {
            var result = await identityClient.Login(user, cancellationToken);
            
            if (result == Services.IdentityResult.Failed)
            {
                return TypedResults.Unauthorized();
            }

            return SignIn(user, result.Token);
        });

        group.MapGet("/login/{provider}", (string provider, string? returnUrl) =>
        {
            var authProperties = GetAuthProperties(returnUrl);
            
            authProperties.SetString(ProviderKey, provider);
            
            return TypedResults.Challenge(
                properties: GetAuthProperties($"identity/signin/{provider}?returnUrl={returnUrl}"),
                authenticationSchemes: [provider]);
        }).AllowAnonymous();

        group.MapPost("/logout", /*async*/ ([FromForm] string? returnUrl/*, HttpContext context*/) => 
        {
            var authProperties = GetAuthProperties(returnUrl);
            
            var provider = authProperties.GetString(ProviderKey);
            
            List<string> schemes = [CookieAuthenticationDefaults.AuthenticationScheme];
            
            if (!string.IsNullOrEmpty(provider))
            {
                schemes.Add(provider);
            }
            
            return TypedResults.SignOut(authProperties, schemes);
            
            //or
            //await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // var result = await context.AuthenticateAsync();
            // var provider = result.Properties?.GetString(ProviderKey);
            //
            // if (provider is not null)
            // {
            //     await context.SignOutAsync(provider, authProperties);
            // }
        });
        
        group.MapGet("signin/{provider}", async (
            string provider, 
            string returnUrl, 
            IUserClient userClient, 
            ISessionClient sessionClient,
            HttpContext context, 
            IDataProtectionProvider dataProtectionProvider) =>
        {
            var result = await context.AuthenticateAsync(IdentityConstants.ExternalScheme);

            if (result is { Succeeded: true, Principal: not null })
            {
                var id = result.Principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
                
                var protector = dataProtectionProvider.CreateProtector(provider);
            
                var providerKey = protector.Protect(id);
                
                var user = await userClient.GetUser(providerKey, new(){ By = UserLookupBy.External, Provider = provider });
                
                if (user is null)
                {
                    return TypedResults.Redirect($"/signin/{provider}/complete?returnUrl={Uri.EscapeDataString(returnUrl)}");
                }
                
                var session = await sessionClient.NewSession(new Session()
                {
                    ProviderKey = providerKey,
                    Username = user.Username
                });
                
                await SignInAsync(context, id, user.Username, session.Token, provider); 
            }
            
            await context.SignOutAsync(IdentityConstants.ExternalScheme);
                
            return TypedResults.Redirect("/");
        });
        
        group.MapPost("signin/{provider}/complete", async (
            string provider,
            [FromForm] string username,
            [FromForm] string returnUrl,
            ISessionClient sessionClient,
            HttpContext context,
            IDataProtectionProvider dataProtectionProvider, 
            CancellationToken cancellationToken = default) =>
        {
            var result = await context.AuthenticateAsync(IdentityConstants.ExternalScheme);

            if (result is not { Succeeded: true, Principal: not null })
            {
                return TypedResults.Redirect("/");
            }

            var id = result.Principal.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var protector = dataProtectionProvider.CreateProtector(provider);
            
            var providerKey = protector.Protect(id);
            
            var session = await sessionClient.NewSession(new Session()
            {
                ProviderKey = providerKey,
                Username = username
            }, cancellationToken);
            
            await SignInAsync(context, id, username, session.Token, provider);
            
            await context.SignOutAsync(IdentityConstants.ExternalScheme);
                
            return TypedResults.Redirect(returnUrl);
        });

        return group;
    }
    
    private static SignInHttpResult SignIn(User user, string bearer)
        => SignIn(user.Email, user.Email, bearer, provider:null, returnUrl:null);
        
    private static SignInHttpResult SignIn(string userId, string username, string token, string? provider, string? returnUrl)
    {
        var (principal, properties) = CreatePrincipalAndProperties(userId, username, token, provider, returnUrl);
        
        return TypedResults.SignIn(
            principal: principal,
            properties: properties,
            authenticationScheme: CookieAuthenticationDefaults.AuthenticationScheme);
    }
    
    private static Task SignInAsync(
        HttpContext context,
        string userId,
        string username,
        string token,
        string? provider,
        string? returnUrl = null)
    {
        var (principal, properties) = CreatePrincipalAndProperties(userId, username, token, provider, returnUrl);

        return context.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            properties);
    }

    private static (ClaimsPrincipal principal, AuthenticationProperties properties) CreatePrincipalAndProperties(
        string userId,
        string username,
        string token,
        string? provider,
        string? returnUrl)
    {
        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId));
        identity.AddClaim(new Claim(ClaimTypes.Name, username));

        var properties = GetAuthProperties(returnUrl);

        if (provider is not null)
        {
            properties.SetString(ProviderKey, provider);
        }

        properties.StoreTokens([
            new AuthenticationToken { Name = "access_token", Value = token }
        ]);

        return (new ClaimsPrincipal(identity), properties);
    }
    
    // Prevent open redirects. Non-empty returnUrls are absolute URIs provided by NavigationManager.Uri.
    static AuthenticationProperties GetAuthProperties(string? returnUrl)
    {
        const string pathBase = "/";

        var safeReturnUrl = returnUrl switch
        {
            null or "" => pathBase,

            // Relative URL -> ensure it starts with '/'
            var url when Uri.IsWellFormedUriString(url, UriKind.Relative)
                => url[0] == '/' ? url : $"{pathBase}{url}",

            // Not a well-formed relative URL -> treat as absolute and keep only PathAndQuery
            var url => new Uri(url, UriKind.Absolute).PathAndQuery
        };

        return new AuthenticationProperties { RedirectUri = safeReturnUrl };
    }
    
    

    
}
