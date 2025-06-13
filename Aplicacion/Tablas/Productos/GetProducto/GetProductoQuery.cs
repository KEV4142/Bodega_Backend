using Aplicacion.Core;
using Aplicacion.Interface;
using Aplicacion.Tablas.Productos.DTOProductos;
using MediatR;

namespace Aplicacion.Tablas.Productos.GetProducto;
public class GetProductoQuery
{
    public record GetProductoQueryRequest : IRequest<Result<ProductoResponse>>
    {
        public int ProductoID { get; set; }
    }
    internal class GetProductoQueryHandler : IRequestHandler<GetProductoQueryRequest, Result<ProductoResponse>>
    {
        private readonly IProductoService _productoService;

        public GetProductoQueryHandler(IProductoService productoService)
        {
            _productoService = productoService;
        }

        public async Task<Result<ProductoResponse>> Handle(
            GetProductoQueryRequest request,
            CancellationToken cancellationToken
        )
        {
            var producto = await _productoService.ObtenerProductoPorIDResponse(request.ProductoID,cancellationToken);

            if (!producto.IsSuccess)
            {
                return Result<ProductoResponse>.Failure(producto.Error!, producto.StatusCode);
            }

            return Result<ProductoResponse>.Success(producto.Value!);
        }


    }
}
