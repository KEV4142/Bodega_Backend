using Aplicacion.Core;
using Aplicacion.Interface;
using Aplicacion.Service;
using Aplicacion.Tablas.Accounts.Login;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace Aplicacion;
public static class DependencyInjection
{
    public static IServiceCollection AddApplicacion(
        this IServiceCollection services
    )
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

            //configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<LoginCommand>();
        // services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddAutoMapper(typeof(MappingProfile).Assembly);
        services.AddScoped<IDistribucionService, DistribucionService>();
        services.AddScoped<IDetalleSalidaValidator, DetalleSalidaValidator>();
        services.AddScoped<IRestriccionSalidaService, RestriccionSalidaService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<ISucursalService, SucursalService>();
        services.AddScoped<IProductoService, ProductoService>();
        services.AddScoped<ILoteService, LoteService>();
        services.AddScoped<IProfileFactory, ProfileFactory>();
        services.AddScoped<IRoleTranslator, RoleTranslator>();
        services.AddScoped<IDistribuidorLotes, DistribuidorLotes>();


        return services;
    }
}