using Microsoft.OpenApi.Models;

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

            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Insira o token JWT: {seu_token}",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };

            options.AddSecurityDefinition("Bearer", securityScheme);
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

        });

        return services;
    }
}