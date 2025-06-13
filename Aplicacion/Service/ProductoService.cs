using System.Net;
using Aplicacion.Core;
using Aplicacion.Interface;
using Aplicacion.Tablas.Productos.DTOProductos;
using AutoMapper;
using Modelo.Entidades;
using Modelo.Interfaces;

namespace Aplicacion.Service;

public class ProductoService : IProductoService
{
    private readonly IProductoRepository _productoRepository;
    private readonly IMapper _mapper;
    public ProductoService(IProductoRepository productoRepository, IMapper mapper)
    {
        _productoRepository = productoRepository;
        _mapper = mapper;
    }
    public async Task<Result<Producto>> ObtenerProductoPorID(int productoID, CancellationToken cancellationToken)
    {
        var producto = await _productoRepository.ObtenerPorIDAsync(productoID, cancellationToken);
        if (producto is null)
            return Result<Producto>.Failure("No se encontró el Producto.", HttpStatusCode.NotFound);

        return Result<Producto>.Success(producto);
    }
    public async Task<Result<ProductoResponse>> ObtenerProductoPorIDResponse(int productoID, CancellationToken cancellationToken)
    {
        var producto = await _productoRepository.ObtenerPorIDAsync(productoID, cancellationToken);
        if (producto is null)
            return Result<ProductoResponse>.Failure("No se encontró el Producto.", HttpStatusCode.NotFound);
        var productoDTO = _mapper.Map<ProductoResponse>(producto);

        return Result<ProductoResponse>.Success(productoDTO);
    }
    public async Task<int> TieneInventarioDisponible(int productoID, CancellationToken cancellationToken)
    {
        var disponible = await _productoRepository.ObtenerInventarioDisponibleAsync(productoID, cancellationToken);

        return disponible;
    }
    public async Task<Result<List<ProductoResponse>>> ObtenerProductosActivos(CancellationToken cancellationToken)
    {
        var productos = await _productoRepository.ObtenerProductosActivosAsync(cancellationToken);
        var productosDTO = _mapper.Map<List<ProductoResponse>>(productos);

        return Result<List<ProductoResponse>>.Success(productosDTO);
    }

}
