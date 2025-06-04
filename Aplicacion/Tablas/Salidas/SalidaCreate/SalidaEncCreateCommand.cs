using System.Net;
using Aplicacion.Core;
using Aplicacion.Interface;
using FluentValidation;
using MediatR;
using Modelo.Entidades;
using Persistencia;

namespace Aplicacion.Tablas.Salidas.SalidaCreate;

public class SalidaEncCreateCommand
{
    public record SalidaEncCreateCommandRequest(SalidaEncCreateRequest salidaEncCreateRequest) : IRequest<Result<int>>;

    internal class SalidaEncCreateCommandHandler : IRequestHandler<SalidaEncCreateCommandRequest, Result<int>>
    {
        private readonly BackendContext _backendContext;
        private readonly IDistribucionService _distribucionService;
        private readonly IDetalleSalidaValidator _detalleSalidaValidator;
        private readonly IRestriccionSalidaService _restriccionSalidaService;
        private readonly IUsuarioService _usuarioService;
        private readonly ISucursalService _sucursalService;

        public SalidaEncCreateCommandHandler(BackendContext backendContext, IDistribucionService distribucionService, IDetalleSalidaValidator detalleSalidaValidator, IRestriccionSalidaService restriccionSalidaService, IUsuarioService usuarioService, ISucursalService sucursalService)
        {
            _backendContext = backendContext;
            _distribucionService = distribucionService;
            _detalleSalidaValidator = detalleSalidaValidator;
            _restriccionSalidaService = restriccionSalidaService;
            _usuarioService = usuarioService;
            _sucursalService = sucursalService;
        }

        public async Task<Result<int>> Handle(SalidaEncCreateCommandRequest request, CancellationToken cancellationToken)
        {
            var usuarioResult = await _usuarioService.ObtenerUsuarioActualAsync();
            if (!usuarioResult.IsSuccess)
                {return Result<int>.Failure(usuarioResult.Error!, usuarioResult.StatusCode);}
            var usuario = usuarioResult.Value!;

            var salidaEnc = new SalidaEnc
            {
                Fecha = DateTime.Now,
                SucursalID = request.salidaEncCreateRequest.SucursalID,
                UsuarioID = usuario.Id
            };
            var sucursalResultado = await _sucursalService.ObtenerSucursalPorIDAsync(request.salidaEncCreateRequest.SucursalID);
            if (!sucursalResultado.IsSuccess)
                {return Result<int>.Failure(sucursalResultado.Error!, HttpStatusCode.NotFound);}

            salidaEnc.Sucursales = sucursalResultado.Value!;

            var distribucion = await _distribucionService.ObtenerDistribucionAsync(request.salidaEncCreateRequest.SalidasDetalle, cancellationToken);
            var lotesDetalle = distribucion.LotesDetalle;
            var lotesValidos = distribucion.LotesValidos;

            var salidasDetalles = new List<SalidaDet>();
            decimal sumaDetalle = 0;

            foreach (var detalle in request.salidaEncCreateRequest.SalidasDetalle)
            {
                var resultadoValidacion = _detalleSalidaValidator.ValidarDetalle(detalle, lotesValidos, lotesDetalle);
                if (!resultadoValidacion.IsSuccess)
                    {return Result<int>.Failure(resultadoValidacion.Error!, HttpStatusCode.BadRequest);}

                var lote = resultadoValidacion.Value!;
                sumaDetalle += lote.Costo * detalle.Cantidad;
                lote.Cantidad -= detalle.Cantidad;

                salidasDetalles.Add(new SalidaDet
                {
                    LoteID = detalle.LoteID,
                    Cantidad = detalle.Cantidad,
                    Lote = lote,
                    Costo = lote.Costo,
                    Salida = salidaEnc
                });
            }

            salidaEnc.SalidaDets = salidasDetalles;

            var restriccionResult = await _restriccionSalidaService.ValidarLimiteSucursal(salidaEnc.SucursalID, sumaDetalle, cancellationToken);
            if (!restriccionResult.IsSuccess)
            {
                return Result<int>.Failure(restriccionResult.Error!, HttpStatusCode.BadRequest);
            }
            _backendContext.Add(salidaEnc);

            try
            {
                using var transaction = await _backendContext.Database.BeginTransactionAsync(cancellationToken);

                await _backendContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Result<int>.Failure("No se pudo insertar el registro de la Orden ni su Detalle.", HttpStatusCode.BadRequest);
            }
            return Result<int>.Success(salidaEnc.SalidaID);
        }
    }


    public class SalidaEncCreateCommandRequesttValidator : AbstractValidator<SalidaEncCreateCommandRequest>
    {
        public SalidaEncCreateCommandRequesttValidator()
        {
            RuleFor(x => x.salidaEncCreateRequest).SetValidator(new SalidaCreateValidator());
        }
    }
}