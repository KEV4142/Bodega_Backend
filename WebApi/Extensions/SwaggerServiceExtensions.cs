using System.Reflection;
using Microsoft.OpenApi.Models;

namespace WebApi.Extensions;
public static class SwaggerServiceExtensions
{
    public static IServiceCollection AddSwaggerDocumentation
    (
        this IServiceCollection services
    )
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            var securitySchema = new OpenApiSecurityScheme
            {
                Description = "JWT Authorization Bearer Schema",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };
            c.AddSecurityDefinition("Bearer", securitySchema);

            var securityRequirement = new OpenApiSecurityRequirement
            {
                {
                    securitySchema, new [] {"Bearer"}
                }
            };

            c.AddSecurityRequirement(securityRequirement);
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });


        return services;
    }
    public static IApplicationBuilder useSwaggerDocumentation(
        this IApplicationBuilder app
    )
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        return app;
    }
}