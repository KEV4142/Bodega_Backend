using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Modelo.Custom;
using Modelo.Entidades;
using Persistencia;

namespace WebApi.Extensions;
public static class DataSeed
{
    public static async Task SeedDataAuthentication(
        this IApplicationBuilder app
    )
    {
        using var scope = app.ApplicationServices.CreateScope();
        var service = scope.ServiceProvider;
        var loggerFactory = service.GetRequiredService<ILoggerFactory>();

        try
        {
            var context = service.GetRequiredService<BackendContext>();
            await context.Database.MigrateAsync();

            var userManager = service.GetRequiredService<UserManager<Usuario>>();

            if (!userManager.Users.Any())
            {
                var userAdmin = new Usuario
                {
                    NombreCompleto = "Juan Bautista",
                    UserName = "JBAUTISTA",
                    Email = "ADMINISTRADOR.PRUEBA@GMAIL.COM"
                };

                await userManager.CreateAsync(userAdmin, "Password123$");
                await userManager.AddToRoleAsync(userAdmin, CustomRoles.ADMINBODEGA);

                var userClient = new Usuario
                {
                    NombreCompleto = "Juan Perez",
                    UserName = "JPEREZ",
                    Email = "juan.perez@gmail.com"
                };

                await userManager.CreateAsync(userClient, "Password123$");
                await userManager.AddToRoleAsync(userClient, CustomRoles.CLIENT);
            }

        }
        catch (Exception e)
        {
            var logger = loggerFactory.CreateLogger<BackendContext>();
            logger.LogError(e.Message);
        }


    }
}