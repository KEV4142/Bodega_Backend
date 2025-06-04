using Aplicacion.Core;
using Modelo.Entidades;

namespace Aplicacion.Interface;

public interface ISucursalService
{
    Task<Result<Sucursal>> ObtenerSucursalPorIDAsync(int sucursalID);
}