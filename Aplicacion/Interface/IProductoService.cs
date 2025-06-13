using Aplicacion.Core;
using Aplicacion.Tablas.Productos.DTOProductos;
using Modelo.Entidades;

namespace Aplicacion.Interface;

public interface IProductoService
{
    Task<Result<Producto>> ObtenerProductoPorID(int productoID, CancellationToken cancellationToken);
    Task<Result<ProductoResponse>> ObtenerProductoPorIDResponse(int productoID, CancellationToken cancellationToken);
    Task<int> TieneInventarioDisponible(int productoID, CancellationToken cancellationToken);
    Task<Result<List<ProductoResponse>>> ObtenerProductosActivos(CancellationToken cancellationToken);

}
