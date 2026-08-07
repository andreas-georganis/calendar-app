using System.Globalization;
using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace CalendarApp.API.RateLimiting;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<TokenBucketRateLimiterOptions>().Configure(options =>
        {
            options.TokenLimit = 200;
            options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            options.QueueLimit = 200;
            options.ReplenishmentPeriod = TimeSpan.FromSeconds(20);
            options.TokensPerPeriod = 200;
            options.AutoReplenishment = true;
        }).Bind(configuration.GetSection("RateLimiting:TokenBucket"));

        services.AddOptions<RateLimiterOptions>().Configure((RateLimiterOptions o, IOptionsMonitor<TokenBucketRateLimiterOptions> monitor) =>
        {
            o.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            o.OnRejected = async (context, cancellationToken) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter =
                        ((int) retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
                }

                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", cancellationToken);
            };
            
            o.AddPolicy("per-user", context =>
            {
                var username = context.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

                return RateLimitPartition.GetTokenBucketLimiter(username, monitor.Get);
            });
        });
        
        return services;
    }
}