using Modelo.Entidades;

namespace Modelo.Interfaces;

public interface IProductoRepository
{
    Task<Producto?> ObtenerPorIDAsync(int productoID, CancellationToken cancellationToken);
    Task<int> ObtenerInventarioDisponibleAsync(int productoID, CancellationToken cancellationToken);
    Task<List<Producto>> ObtenerProductosActivosAsync(CancellationToken cancellationToken);

}
