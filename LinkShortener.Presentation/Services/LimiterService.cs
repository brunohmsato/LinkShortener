using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace LinkShortener.Presentation.Services;

public static class LimiterService
{
    public static IServiceCollection AddLimiterServices(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("create-link", opt =>
            {
                opt.Window = TimeSpan.FromMinutes(1);
                opt.PermitLimit = 20;
                opt.QueueLimit = 0;
            });

            options.AddPolicy("dynamic-policy", httpContext =>
            {
                if (httpContext.User.Identity?.IsAuthenticated == true)
                {
                    var userId = httpContext.User.FindFirst("idUsuario")?.Value;

                    if (!string.IsNullOrEmpty(userId))
                    {
                        return RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey: userId,
                            factory: _ => new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = 10,
                                Window = TimeSpan.FromMinutes(1),
                            });
                    }
                }

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: "global",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 4,
                        Window = TimeSpan.FromSeconds(12),
                    });
            });
        });

        return services;
    }
}