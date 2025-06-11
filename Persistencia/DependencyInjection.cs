using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modelo.Interfaces;
using Persistencia.Repositorios;

namespace Persistencia;
public static class DependencyInjection
{
    public static IServiceCollection AddPersistencia(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<BackendContext>(opt =>
        {
            opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });
        services.AddScoped<ISucursalRepository, SucursalRepository>();
        services.AddScoped<IProductoRepository, ProductoRepository>();
        services.AddScoped<ILoteRepository, LoteRepository>();
        services.AddScoped<ISalidaEncRepository, SalidaEncRepository>();
        return services;
    }
}