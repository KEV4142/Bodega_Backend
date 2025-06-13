using Aplicacion.Core;
using Aplicacion.Interface;
using Aplicacion.Tablas.Productos.DTOProductos;
using MediatR;


namespace Aplicacion.Tablas.Productos.GetProductosActivos;
public class GetProductosActivos
{
    public record GetProductosActivasQueryRequest : IRequest<Result<List<ProductoResponse>>>
    {
    }
    internal class GetProductosActivasQueryHandler
        : IRequestHandler<GetProductosActivasQueryRequest, Result<List<ProductoResponse>>>
    {
        private readonly IProductoService _productoService;

        public GetProductosActivasQueryHandler(IProductoService productoService)
        {
            _productoService = productoService;
        }

        public async Task<Result<List<ProductoResponse>>> Handle(
            GetProductosActivasQueryRequest request, 
            CancellationToken cancellationToken
        )
        {
            var productoListado = await _productoService.ObtenerProductosActivos(cancellationToken);
            return productoListado;
        }
    }
}