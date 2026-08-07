using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Yarp.ReverseProxy.Forwarder;
using Yarp.ReverseProxy.Transforms;

namespace Calendar.Web.Apis;

public static class YarpExtensions
{
    internal static IEndpointConventionBuilder MapAuthorizedForwarder(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        string destinationPrefix = "",
        ForwarderRequestConfig? requestConfig = null)
    {
        requestConfig ??= new ForwarderRequestConfig();

        return endpoints.MapForwarder(pattern, destinationPrefix, requestConfig, transforms =>
        {
            transforms.AddRequestTransform(async ctx =>
            {
                var accessToken = await ctx.HttpContext.GetTokenAsync("access_token");

                if (accessToken is null)
                {
                    return;
                }

                ctx.ProxyRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            });
        });
    }
    
}