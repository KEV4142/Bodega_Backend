using Modelo.Custom;

namespace WebApi.Extensions;
public static class PoliciesConfiguration
{
    public static IServiceCollection AddPoliciesServices(
        this IServiceCollection services
    )
    {
        services.AddAuthorization(opt =>
        {
            opt.AddPolicy(
                CustomRoles.ADMINBODEGA, policy =>
                   policy.RequireRole(CustomRoles.ADMINBODEGA)
            );
            opt.AddPolicy(
                CustomRoles.CLIENT,
                policy => policy.RequireRole(CustomRoles.CLIENT)
            );
        });
        return services;
    }
}