using System.Net;
using Aplicacion.Core;
using Aplicacion.Interface;
using Aplicacion.Tablas.Lotes.DTOLotes;
using FluentValidation;
using MediatR;


namespace Aplicacion.Tablas.Lotes.GetLotesSalida;
public class GetLotesSalidaQuery
{
    public record GetLotesSalidaQueryRequest(GetLotesSalidaRequest getLotesSalidaRequest) : IRequest<Result<List<LoteCompletoResponse>>>;

    internal class GetLotesSalidaQueryHandler
        : IRequestHandler<GetLotesSalidaQueryRequest, Result<List<LoteCompletoResponse>>>
    {
        private readonly IDistribuidorLotes _distribuidorLotes;
        private readonly IProductoService _productoService;
        private readonly ILoteService _loteService;

        public GetLotesSalidaQueryHandler(IDistribuidorLotes distribuidorLotes, IProductoService productoService, ILoteService loteService)
        {
            _distribuidorLotes = distribuidorLotes;
            _productoService = productoService;
            _loteService = loteService;
        }

        public async Task<Result<List<LoteCompletoResponse>>> Handle(
            GetLotesSalidaQueryRequest request,
            CancellationToken cancellationToken
        )
        {
            var productoResultado = await _productoService.ObtenerProductoPorID(request.getLotesSalidaRequest.ProductoID, cancellationToken);
            if (!productoResultado.IsSuccess)
                {return Result<List<LoteCompletoResponse>>.Failure(productoResultado.Error!, productoResultado.StatusCode);}

            var productoDisponible = await _productoService.TieneInventarioDisponible(request.getLotesSalidaRequest.ProductoID,cancellationToken);

            if (productoDisponible < request.getLotesSalidaRequest.Cantidad)
            {
                return Result<List<LoteCompletoResponse>>.Failure($"No se tiene suficiente Inventario para la Salida({productoDisponible}).", HttpStatusCode.BadRequest);
            }

            var productosListado = await _loteService.ObtenerLotesDisponiblesOrdenados(request.getLotesSalidaRequest.ProductoID, cancellationToken);

            var productosSalida = new List<LoteCompletoResponse>();

            productosSalida = _distribuidorLotes.Distribuir(
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