using System.Net;
using Aplicacion.Core;
using Aplicacion.Interface;
using Microsoft.EntityFrameworkCore;
using Modelo.Entidades;
using Persistencia;

namespace Aplicacion.Service;

public class ProductoService : IProductoService
{
    private readonly BackendContext _backendContext;
    public ProductoService(BackendContext context)
    {
        _backendContext = context;
    }
    public async Task<Result<Producto>> ObtenerProductoPorIDAsync(int productoID)
    {
        var producto = await _backendContext.Productos!.FirstOrDefaultAsync(p => p.ProductoID == productoID);
        if (producto is null)
            return Result<Producto>.Failure("No se encontró el Producto.", HttpStatusCode.NotFound);

        return Result<Producto>.Success(producto);
    }
    public async Task<int> TieneInventarioDisponible(int productoID, int cantidad)
    {
        var disponible = await _backendContext.Lotes!
            .Where(l => l.ProductoID == productoID)
            .SumAsync(l => (int?)l.Cantidad) ?? 0;

        return disponible;
    }
}
