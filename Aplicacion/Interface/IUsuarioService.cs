using Aplicacion.Core;
using Modelo.Entidades;

namespace Aplicacion.Interface;

public interface IUsuarioService
{
    Task<Result<Usuario>> ObtenerUsuarioActualAsync();
}
