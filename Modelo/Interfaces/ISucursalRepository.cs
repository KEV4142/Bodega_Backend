using Modelo.Entidades;

namespace Modelo.Interfaces;

public interface ISucursalRepository
{
    Task<Sucursal?> ObtenerPorIDAsync(int sucursalID);
    Task<List<Sucursal>> ObtenerSucursalesActivasAsync(CancellationToken cancellationToken);

}
