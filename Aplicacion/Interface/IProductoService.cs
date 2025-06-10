using Aplicacion.Core;
using Modelo.Entidades;

namespace Aplicacion.Interface;

public interface IProductoService
{
    Task<Result<Producto>> ObtenerProductoPorID(int productoID);
    Task<int> TieneInventarioDisponible(int productoID, int cantidad);
    Task<List<Producto>> ObtenerProductosActivos(CancellationToken cancellationToken);

}
