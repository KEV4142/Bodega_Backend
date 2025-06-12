using Modelo.Entidades;

namespace Modelo.Interfaces;

public interface ILoteRepository
{
    Task<List<Lote>> ObtenerLotesDisponiblesOrdenadosAsync(int productoID);
    Task<List<Lote>> ObtenerLotesPorIDListaAsync(List<int> loteIds, CancellationToken cancellationToken);
    Task<List<Lote>> ObtenerLotesDisponiblesParaProductoAsync(int productoId, CancellationToken cancellationToken);
}
