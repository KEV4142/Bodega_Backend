using Modelo.Entidades;

namespace Modelo.Interfaces;

public interface IProductoRepository
{
    Task<Producto?> ObtenerPorIDAsync(int productoID);
    Task<int> ObtenerInventarioDisponibleAsync(int productoID);
    Task<List<Producto>> ObtenerProductosActivosAsync(CancellationToken cancellationToken);

}
