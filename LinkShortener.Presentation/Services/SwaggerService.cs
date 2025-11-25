
using Microsoft.OpenApi;

namespace LinkShortener.Presentation.Services;

public static class SwaggerService
{
    public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "LinkShortener",
                Version = "v1",
                //Description = "",
                //Contact = new OpenApiContact
                //{
                //    Name = "",
                //    Email = "email@empresa.com",
                //    Url = new Uri("https://www.fenoxtec.com.br")
                //},
                //License = new OpenApiLicense
                //{
                //    Name = "MIT",
                //    Url = new Uri("https://opensource.org/licenses/MIT")
                //},
                //TermsOfService = new Uri("https://www.seusite.com/termos")
            });

            options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Insira o token JWT: {seu_token}",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("bearer", document)] = []
            });

        });

        return services;
    }
}