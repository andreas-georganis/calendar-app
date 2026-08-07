using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace Calendar.Web.Authentication;

public class ExternalAuthProviders(IAuthenticationSchemeProvider schemeProvider)   
{
    public async Task<IEnumerable<string>> GetSchemesAsync()
    {
        var schemes = await schemeProvider.GetAllSchemesAsync();// also from signin manager, if I had the dependency
        
        var externalSchemes = schemes
            .Where(x => x.Name is not CookieAuthenticationDefaults.AuthenticationScheme)
            .Where(x => x.Name != IdentityConstants.ExternalScheme)
            .Select(x => x.Name);   

        return externalSchemes;
    }
}