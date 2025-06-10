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
}
