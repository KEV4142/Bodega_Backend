using Microsoft.EntityFrameworkCore;
using Modelo.Entidades;
using Modelo.Interfaces;

namespace Persistencia.Repositorios;

public class ProductoRepository : IProductoRepository
{
    private readonly BackendContext _backendContext;

    public ProductoRepository(BackendContext context)
    {
        _backendContext = context;
    }
    public async Task<Producto?> ObtenerPorIDAsync(int productoID)
    {
        return await _backendContext.Productos!.FirstOrDefaultAsync(p => p.ProductoID == productoID);
    }
    public async Task<int> ObtenerInventarioDisponibleAsync(int productoID)
    {
        return await _backendContext.Lotes!
            .Where(l => l.ProductoID == productoID)
            .SumAsync(l => (int?)l.Cantidad) ?? 0;
    }
    public async Task<List<Producto>> ObtenerProductosActivosAsync(CancellationToken cancellationToken)
    {
        return await _backendContext.Productos!
            .Where(s => s.Estado != null && s.Estado.ToUpper().Equals("A"))
            .OrderBy(c => c.ProductoID)
            .ToListAsync(cancellationToken);
    }
}
