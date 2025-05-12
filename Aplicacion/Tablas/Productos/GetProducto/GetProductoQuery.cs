using System.Net;
using Aplicacion.Core;
using Aplicacion.Tablas.Productos.DTOProductos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistencia;



namespace Aplicacion.Tablas.Productos.GetProducto;
public class GetProductoQuery
{
    public record GetProductoQueryRequest : IRequest<Result<ProductoResponse>>
    {
        public int ProductoID { get; set; }
    }
    internal class GetProductoQueryHandler : IRequestHandler<GetProductoQueryRequest, Result<ProductoResponse>>
    {
        private readonly BackendContext _context;
        private readonly IMapper _mapper;

        public GetProductoQueryHandler(BackendContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<ProductoResponse>> Handle(
            GetProductoQueryRequest request,
            CancellationToken cancellationToken
        )
        {
            var producto = await _context.Productos!.Where(x => x.ProductoID == request.ProductoID)
            .ProjectTo<ProductoResponse>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

            if (producto is null)
            {
                return Result<ProductoResponse>.Failure("No se encontro el Producto.", HttpStatusCode.NotFound);
            }

            return Result<ProductoResponse>.Success(producto!);
        }


    }
}
