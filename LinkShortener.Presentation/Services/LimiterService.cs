using System.Threading.RateLimiting;

namespace LinkShortener.Presentation.Services;

public static class LimiterService
{
    public static IServiceCollection AddLimiterServices(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    }));

            options.OnRejected = (context, token) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.WriteAsync("Too many requests. Try again later.");
                return ValueTask.CompletedTask;
            };

            options.AddPolicy("dynamic-policy", httpContext =>
            {
                if (httpContext.User.Identity?.IsAuthenticated == true)
                {
                    var userId = httpContext.User.FindFirst("jti")?.Value;

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