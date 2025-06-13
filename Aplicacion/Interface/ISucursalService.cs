using Aplicacion.Core;
using Aplicacion.Tablas.Sucursales.DTOSucursales;
using Modelo.Entidades;

namespace Aplicacion.Interface;

public interface ISucursalService
{
    Task<Result<Sucursal>> ObtenerSucursalPorID(int sucursalID, CancellationToken cancellationToken);
    Task<Result<SucursalResponse>> ObtenerSucursalPorIDResponse(int sucursalID, CancellationToken cancellationToken);
    Task<Result<List<SucursalResponse>>> ObtenerSucursalesActivas(CancellationToken cancellationToken);

}