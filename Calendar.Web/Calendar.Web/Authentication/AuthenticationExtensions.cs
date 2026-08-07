using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Calendar.Web.Authentication;


public static class AuthenticationExtensions
{
    public static AuthenticationBuilder AddExternalProviders(
        this AuthenticationBuilder builder,
        IConfiguration config)
    {
        var root = config.GetSection("Authentication:Schemes");
        if (!root.Exists()) return builder;

        AddGitHub(builder, root.GetSection("GitHub")); // OAuth-only
        AddGoogleOidc(builder, root.GetSection("Google")); // OIDC
        AddMicrosoft(builder,
            root.GetSection("Microsoft")); // OIDC if Tenant/Authority present; else MSA OAuth fallback
        AddAuth0Oidc(builder, root.GetSection("Auth0")); // OIDC

        return builder;
    }

    private static void AddGitHub(AuthenticationBuilder builder, IConfigurationSection s)
    {
        if (!s.Exists()) return;

        // Requires package: AspNet.Security.OAuth.GitHub (aspnet-contrib)
        builder.AddGitHub("GitHub", o =>
        {
            o.SignInScheme = IdentityConstants.ExternalScheme;
            o.ClientId = Require(s, "ClientId");
            o.ClientSecret = Require(s, "ClientSecret");

            // commonly needed to get a reliable email
            o.Scope.Add("user:email");

            var cb = s["CallbackPath"];
            if (!string.IsNullOrWhiteSpace(cb)) o.CallbackPath = cb;
        });
    }

    private static void AddGoogleOidc(AuthenticationBuilder builder, IConfigurationSection s)
    {
        if (!s.Exists()) return;

        builder.AddOpenIdConnect("Google", "Google", o =>
        {
            o.SignInScheme = IdentityConstants.ExternalScheme;

            // Google OIDC issuer/authority
            o.Authority =
                "https://accounts.google.com"; // uses /.well-known/openid-configuration  [oai_citation:3‡Google for Developers](https://developers.google.com/identity/openid-connect/openid-connect?utm_source=chatgpt.com)

            o.ClientId = Require(s, "ClientId");
            o.ClientSecret = Require(s, "ClientSecret");
            o.ResponseType = OpenIdConnectResponseType.Code;

            o.Scope.Clear();
            o.Scope.Add("openid");
            o.Scope.Add("profile");
            o.Scope.Add("email");

            o.GetClaimsFromUserInfoEndpoint = true;
            o.SaveTokens = true;

            o.CallbackPath = s["CallbackPath"] ?? "/signin-google-oidc";
        });
    }

    private static void AddMicrosoft(AuthenticationBuilder builder, IConfigurationSection s)
    {
        if (!s.Exists()) return;

        // If you provide Authority or TenantId, we treat it as Entra (OIDC).
        // Otherwise we fall back to MicrosoftAccount (OAuth) if you still want that behavior.
        var authority = s["Authority"];
        var tenantId = s["TenantId"];

        if (!string.IsNullOrWhiteSpace(authority) || !string.IsNullOrWhiteSpace(tenantId))
        {
            var effectiveAuthority = !string.IsNullOrWhiteSpace(authority)
                ? authority
                : $"https://login.microsoftonline.com/{tenantId}/v2.0";

            builder.AddOpenIdConnect("Microsoft", "Microsoft (Work/School)", o =>
            {
                o.SignInScheme = IdentityConstants.ExternalScheme;
                o.Authority =
                    effectiveAuthority; // OIDC code flow recommended  [oai_citation:4‡Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/configure-oidc-web-authentication?view=aspnetcore-10.0&utm_source=chatgpt.com)

                o.ClientId = Require(s, "ClientId");
                o.ClientSecret = Require(s, "ClientSecret");
                o.ResponseType = OpenIdConnectResponseType.Code;

                o.Scope.Clear();
                o.Scope.Add("openid");
                o.Scope.Add("profile");
                o.Scope.Add("email");

                // Good defaults
                o.SaveTokens = true;
                o.CallbackPath = s["CallbackPath"] ?? "/signin-entra";

                // Often useful for Identity Name/Email mapping:
                o.TokenValidationParameters.NameClaimType = "name";
            });

            return;
        }

        // Fallback: Microsoft personal accounts via MicrosoftAccount OAuth handler  [oai_citation:5‡Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/microsoft-logins?view=aspnetcore-10.0&utm_source=chatgpt.com)
        builder.AddMicrosoftAccount("Microsoft", "Microsoft (Personal)", o =>
        {
            o.SignInScheme = IdentityConstants.ExternalScheme;
            o.ClientId = Require(s, "ClientId");
            o.ClientSecret = Require(s, "ClientSecret");

            var cb = s["CallbackPath"];
            if (!string.IsNullOrWhiteSpace(cb)) o.CallbackPath = cb;
        });
    }
    
    private static void AddAuth0(AuthenticationBuilder builder, IConfigurationSection s)
    {
        if (!s.Exists()) return;

        var useSdk = s.GetValue("UseSdk", false);

        var domain = Require(s, "Domain");
        var clientId = Require(s, "ClientId");
        var clientSecret = Require(s, "ClientSecret");
        var callbackPath = s["CallbackPath"] ?? "/signin-auth0";
        var scope = s["Scope"] ?? "openid profile email";
        var audience = s["Audience"];
        
        if (useSdk)
        {
            // Auth0 SDK (still OIDC underneath)  [oai_citation:2‡Auth0](https://auth0.com/blog/exploring-auth0-aspnet-core-authentication-sdk/?utm_source=chatgpt.com)
            var auth0 = builder.AddAuth0WebAppAuthentication("Auth0", options =>
            {
                options.Domain = domain;
                options.ClientId = clientId;
                options.ClientSecret = clientSecret;
                options.Scope = scope;
                options.CallbackPath = callbackPath;

                // IMPORTANT for ASP.NET Core Identity external login flow:
                // stage the external principal in Identity.External
                options.SignInScheme = IdentityConstants.ExternalScheme; // property exists  [oai_citation:3‡auth0.github.io](https://auth0.github.io/auth0-aspnetcore-authentication/api/Auth0.AspNetCore.Authentication.Auth0WebAppOptions.html?utm_source=chatgpt.com)

                // You already configure cookies via Identity; avoid SDK adding its own cookie middleware
                options.SkipCookieMiddleware = true; //  [oai_citation:4‡auth0.github.io](https://auth0.github.io/auth0-aspnetcore-authentication/api/Auth0.AspNetCore.Authentication.Auth0WebAppOptions.html?utm_source=chatgpt.com)
            });

            // Optional access token for an API (Auth0 SDK convenience)
            if (!string.IsNullOrWhiteSpace(audience))
            {
                auth0.WithAccessToken(o => o.Audience = audience);
            }

            // Extra safety: force the underlying OpenIdConnect handler’s SignInScheme too,
            // because some SDK versions/paths may ignore the wrapper option (David Fowler’s pattern).  [oai_citation:5‡GitHub](https://github.com/davidfowl/TodoApp/blob/main/Todo.Web/Server/Authentication/AuthenticationExtensions.cs?utm_source=chatgpt.com)
            builder.Services.PostConfigure<OpenIdConnectOptions>("Auth0", o =>
            {
                o.SignInScheme = IdentityConstants.ExternalScheme;
            });

            return;
        }

        // Pure OIDC approach
        builder.AddOpenIdConnect("Auth0", "Auth0", o =>
        {
            o.SignInScheme = IdentityConstants.ExternalScheme;
            o.Authority = $"https://{domain}";
            o.ClientId = clientId;
            o.ClientSecret = clientSecret;
            o.CallbackPath = callbackPath;
            o.ResponseType = OpenIdConnectResponseType.Code;

            o.Scope.Clear();
            foreach (var item in scope.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                o.Scope.Add(item);

            o.SaveTokens = true;

            // If you need Auth0 API audience, add it as an extra authorize parameter.
            if (!string.IsNullOrWhiteSpace(audience))
            {
                o.Events = new OpenIdConnectEvents
                {
                    OnRedirectToIdentityProvider = ctx =>
                    {
                        ctx.ProtocolMessage.SetParameter("audience", audience);
                        return Task.CompletedTask;
                    }
                };
            }
        });
    }

    private static void AddAuth0Oidc(AuthenticationBuilder builder, IConfigurationSection s)
    {
        if (!s.Exists()) return;

        var domain = Require(s, "Domain");
        var audience = s["Audience"];
        var scope = s["Scope"] ?? "openid profile email";
        
        builder.AddOpenIdConnect("Auth0", "Auth0", o =>
        {
            o.SignInScheme = IdentityConstants.ExternalScheme;

            // Auth0 issuer is your tenant domain
            o.Authority = $"https://{domain}";
            o.ClientId = Require(s, "ClientId");
            o.ClientSecret = Require(s, "ClientSecret");
            o.ResponseType = OpenIdConnectResponseType.Code;

            o.Scope.Clear();
            foreach (var item in scope.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                o.Scope.Add(item);

            o.SaveTokens = true;
            o.CallbackPath = s["CallbackPath"] ?? "/signin-auth0";

            // Auth0 API access commonly needs "audience" as an extra parameter;
            // OIDC middleware doesn't have a first-class property for it, so set it in the redirect event.  [oai_citation:6‡Auth0](https://auth0.com/blog/backend-for-frontend-pattern-with-auth0-and-dotnet/?utm_source=chatgpt.com)
            if (!string.IsNullOrWhiteSpace(audience))
            {
                o.Events = new OpenIdConnectEvents
                {
                    OnRedirectToIdentityProvider = ctx =>
                    {
                        ctx.ProtocolMessage.SetParameter("audience", audience);
                        return Task.CompletedTask;
                    }
                };
            }
        });

        // Note: Auth0’s ASP.NET Core SDK is just a wrapper around OpenIdConnect and can be “easier”,
        // but the above is the pure AddOpenIdConnect approach.  [oai_citation:7‡Auth0](https://auth0.com/blog/exploring-auth0-aspnet-core-authentication-sdk/?utm_source=chatgpt.com)
    }

    private static string Require(IConfigurationSection s, string key)
        => s[key] ?? throw new InvalidOperationException($"Missing '{s.Path}:{key}'.");
}