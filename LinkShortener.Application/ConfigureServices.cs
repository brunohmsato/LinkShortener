using LinkShortener.Application.Interfaces;
using LinkShortener.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LinkShortener.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddAplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ILinkService, LinkService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IQrCodeService, QrCodeService>();

        return services;
    }
}