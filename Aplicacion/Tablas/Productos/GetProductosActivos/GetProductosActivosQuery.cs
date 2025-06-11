using Aplicacion.Core;
using Aplicacion.Interface;
using Aplicacion.Tablas.Productos.DTOProductos;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public GetProductosActivasQueryHandler(IProductoService productoService, IMapper mapper)
        {
            _productoService = productoService;
            _mapper = mapper;
        }

        public async Task<Result<List<ProductoResponse>>> Handle(
            GetProductosActivasQueryRequest request, 
            CancellationToken cancellationToken
        )
        {
            var productoListado = await _productoService.ObtenerProductosActivos(cancellationToken);
            var response = _mapper.Map<List<ProductoResponse>>(productoListado);

            return Result<List<ProductoResponse>>.Success(response);
        }
    }
}