using System.Net;
using Aplicacion.Core;
using Aplicacion.Tablas.Lotes.DTOLotes;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistencia;

namespace Aplicacion.Tablas.Lotes.GetLotesSalida;
public class GetLotesSalidaQuery
{
    public record GetLotesSalidaQueryRequest(GetLotesSalidaRequest getLotesSalidaRequest) : IRequest<Result<List<LoteCompletoResponse>>>;

    internal class GetLotesSalidaQueryHandler
        : IRequestHandler<GetLotesSalidaQueryRequest, Result<List<LoteCompletoResponse>>>
    {
        private readonly BackendContext _context;
        private readonly IMapper _mapper;

        public GetLotesSalidaQueryHandler(BackendContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<List<LoteCompletoResponse>>> Handle(
            GetLotesSalidaQueryRequest request,
            CancellationToken cancellationToken
        )
        {
            var producto = await _context.Productos!.Where(x => x.ProductoID == request.getLotesSalidaRequest.ProductoID)
            .FirstOrDefaultAsync();

            if (producto is null)
            {
                return Result<List<LoteCompletoResponse>>.Failure("No se encontro el Producto.", HttpStatusCode.NotFound);
            }

            var productoDisponible = await _context.Lotes!
                .Where(l => l.ProductoID == request.getLotesSalidaRequest.ProductoID)
                .SumAsync(l => (int?)l.Cantidad) ?? 0;

            if (productoDisponible < request.getLotesSalidaRequest.Cantidad)
            {
                return Result<List<LoteCompletoResponse>>.Failure("No se tiene suficiente Inventario para la Salida.", HttpStatusCode.BadRequest);
            }

            var productosListado = await _context.Lotes!
                .Where(l => l.ProductoID == request.getLotesSalidaRequest.ProductoID && l.Cantidad > 0)
                .OrderBy(l => l.FechaVencimiento)
                .ProjectTo<LoteCompletoResponse>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            var productosSalida = new List<LoteCompletoResponse>();

            productosSalida = DistribucionLotes.Distribuir(
                                productosListado,
                                request.getLotesSalidaRequest.Cantidad,
                                l => l.Cantidad,
                                (l, nuevaCantidad) => l.Cantidad = nuevaCantidad
                            );

            return Result<List<LoteCompletoResponse>>.Success(productosSalida);
        }
    }


    public class GetLotesSalidaQueryRequestValidator : AbstractValidator<GetLotesSalidaQueryRequest>
    {
        public GetLotesSalidaQueryRequestValidator()
        {
            RuleFor(x => x.getLotesSalidaRequest).SetValidator(new GetLotesSalidaValidator());
        }
    }
}