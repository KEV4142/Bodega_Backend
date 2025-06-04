using Aplicacion.Tablas.Accounts;
using Modelo.Entidades;

namespace Aplicacion.Interface;

public interface IProfileFactory
{
    Task<Profile> CrearAsync(Usuario usuario);
}
