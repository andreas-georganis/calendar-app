using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Calendar.API.OpenApi;

internal sealed class BearerOpenApiTransformer(
    IAuthenticationSchemeProvider authenticationSchemeProvider
) : IOpenApiDocumentTransformer, IOpenApiOperationTransformer
{
    // Add Bearer scheme at the document level
    public async Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var schemes = await authenticationSchemeProvider.GetAllSchemesAsync();

        var hasBearer = schemes.Any(s =>
            string.Equals(s.Name, JwtBearerDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(s.Name, IdentityConstants.BearerScheme, StringComparison.OrdinalIgnoreCase));

        if (!hasBearer)
            return;

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Enter: Bearer {token}"
        };
    }

    // Apply security requirement to operations that require authorization
    public Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        var metadata = context.Description.ActionDescriptor?.EndpointMetadata ?? Array.Empty<object>();

        // Detect anonymous endpoints
        var isAnonymous = metadata.Any(m => m is IAllowAnonymous or IAllowAnonymousFilter);

        // Detect authorized endpoints
        var requiresAuth = !isAnonymous && metadata.Any(m => m is IAuthorizeData);

        if (!requiresAuth)
            return Task.CompletedTask;

        operation.Security ??= new List<OpenApiSecurityRequirement>();

        // Add the Bearer security requirement
        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", context.Document)] = new List<string>()
        });

        return Task.CompletedTask;
    }
}
