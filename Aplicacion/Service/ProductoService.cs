using System.Net;
using Aplicacion.Core;
using Aplicacion.Interface;
using Modelo.Entidades;
using Modelo.Interfaces;

namespace Aplicacion.Service;

public class ProductoService : IProductoService
{
    private readonly IProductoRepository _productoRepository;
    public ProductoService(IProductoRepository productoRepository)
    {
        _productoRepository = productoRepository;
    }
    public async Task<Result<Producto>> ObtenerProductoPorID(int productoID)
    {
        var producto = await _productoRepository.ObtenerPorIDAsync(productoID);
        if (producto is null)
            return Result<Producto>.Failure("No se encontró el Producto.", HttpStatusCode.NotFound);

        return Result<Producto>.Success(producto);
    }
    public async Task<int> TieneInventarioDisponible(int productoID, int cantidad)
    {
        var disponible = await _productoRepository.ObtenerInventarioDisponibleAsync(productoID);

        return disponible;
    }
    public async Task<List<Producto>> ObtenerProductosActivos(CancellationToken cancellationToken)
    {
        return await _productoRepository.ObtenerProductosActivosAsync(cancellationToken);
    }

}
