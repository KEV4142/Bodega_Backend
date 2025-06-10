using Aplicacion.Core;
using Aplicacion.Interface;
using Aplicacion.Tablas.Productos.DTOProductos;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public GetProductoQueryHandler(IProductoService productoService, IMapper mapper)
        {
            _productoService = productoService;
            _mapper = mapper;
        }

        public async Task<Result<ProductoResponse>> Handle(
            GetProductoQueryRequest request,
            CancellationToken cancellationToken
        )
        {
            var producto = await _productoService.ObtenerProductoPorID(request.ProductoID);

            if (!producto.IsSuccess)
            {
                return Result<ProductoResponse>.Failure(producto.Error!, producto.StatusCode);
            }
            var productoDTO = _mapper.Map<ProductoResponse>(producto.Value);

            return Result<ProductoResponse>.Success(productoDTO);
        }


    }
}
