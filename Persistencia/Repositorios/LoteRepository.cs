using Microsoft.EntityFrameworkCore;
using Modelo.Entidades;
using Modelo.Interfaces;

namespace Persistencia.Repositorios;

public class LoteRepository : ILoteRepository
{
    private readonly BackendContext _backendContext;

    public LoteRepository(BackendContext context)
    {
        _backendContext = context;
    }
    public async Task<List<Lote>> ObtenerLotesDisponiblesOrdenadosAsync(int productoID)
    {
        return await _backendContext.Lotes!
            .Where(lote => lote.ProductoID == productoID && lote.Cantidad > 0)
            .OrderBy(l => l.FechaVencimiento)
            .ToListAsync();
    }
    public async Task<List<Lote>> ObtenerLotesPorIDListaAsync(List<int> loteIDs, CancellationToken cancellationToken)
    {
        return await _backendContext.Lotes!.Where(l => loteIDs.Contains(l.LoteID)).ToListAsync(cancellationToken);
    }
    public async Task<List<Lote>> ObtenerLotesDisponiblesParaProductoAsync(int productoID, CancellationToken cancellationToken)
    {
        return await _backendContext.Lotes!
            .Where(l => l.ProductoID == productoID && l.Cantidad > 0)
            .OrderBy(l => l.FechaVencimiento)
            .ToListAsync(cancellationToken);
    }
}
