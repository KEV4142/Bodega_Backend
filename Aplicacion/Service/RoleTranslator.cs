using Aplicacion.Interface;
using Modelo.Custom;

namespace Aplicacion.Service;

public class RoleTranslator : IRoleTranslator
{
    public string ObtenerRol(string? roleName)
    {
        return roleName switch
        {
            CustomRoles.ADMINBODEGA => "Administrador",
            CustomRoles.CLIENT => "Operador",
            _ => "Sin Asignacion de Rol."
        };
    }
}
