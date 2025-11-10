using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Keycloak.AuthServices.Sdk;

namespace LinkShortener.Presentation.Services;

public static class AuthService
{
    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddKeycloakWebApiAuthentication(configuration);

        services.AddKeycloakAuthorization(configuration);

        services.AddKeycloakAdminHttpClient(configuration);

        return services;
    }
}