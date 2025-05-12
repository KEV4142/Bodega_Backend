using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        return services;
    }
}