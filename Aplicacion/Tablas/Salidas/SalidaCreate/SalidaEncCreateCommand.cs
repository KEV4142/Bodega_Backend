using System.Net;
using Aplicacion.Core;
using Aplicacion.Interface;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<SalidaEncCreateCommandHandler> _logger;

        public SalidaEncCreateCommandHandler(BackendContext backendContext, IDistribucionService distribucionService, IDetalleSalidaValidator detalleSalidaValidator, IRestriccionSalidaService restriccionSalidaService, IUsuarioService usuarioService, ISucursalService sucursalService,ILogger<SalidaEncCreateCommandHandler> logger)
        {
            _backendContext = backendContext;
            _distribucionService = distribucionService;
            _detalleSalidaValidator = detalleSalidaValidator;
            _restriccionSalidaService = restriccionSalidaService;
            _usuarioService = usuarioService;
            _sucursalService = sucursalService;
            _logger = logger;
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
                    {return Result<int>.Failure(resultadoValidacion.Error!, resultadoValidacion.StatusCode);}

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
            catch (DbUpdateConcurrencyException ex)
            {
                var conflictoLotes = ex.Entries
                    .Where(e => e.Entity is Lote)
                    .Select(e => (Lote)e.Entity)
                    .ToList();

                var loteIDS = string.Join(",", conflictoLotes.Select(l => l.LoteID));
                var productoIDS = string.Join(",", salidaEnc.SalidaDets.Select(d => d.Lote.ProductoID));
                var cantidades = string.Join(",", salidaEnc.SalidaDets.Select(d => d.Cantidad));

                _logger.LogError(ex, "Conflicto de concurrencia al guardar la salida (SucursalID: {SucursalID}, UsuarioID: {UsuarioID}, LoteIDs: {LoteIDS}, ProductoIDs: {ProductoIDS}, Cantidades: {Cantidades})",
                    salidaEnc.SucursalID, salidaEnc.UsuarioID, loteIDS, productoIDS, cantidades);

                return Result<int>.Failure("Conflicto de concurrencia en lote(s): {loteIds}. Intenta nuevamente rectificando el lote.", HttpStatusCode.Conflict);
            }
            catch (Exception ex)
            {
                var loteIDS = string.Join(",", salidaEnc.SalidaDets.Select(detalle => detalle.LoteID));
                var productoIDS = string.Join(",", salidaEnc.SalidaDets.Select(detalle => detalle.Lote.ProductoID));
                var cantidades = string.Join(",", salidaEnc.SalidaDets.Select(detalle => detalle.Cantidad));

                _logger.LogError(ex, "Error al guardar la salida (SucursalID: {SucursalID}, UsuarioID: {UsuarioID}, LoteID's: {LoteIDS}, ProductoID's: {productoIDS}, Cantidad: {cantidades}) en SalidaEncCreateCommand",
                    salidaEnc.SucursalID,
                    salidaEnc.UsuarioID,
                    loteIDS,
                    productoIDS,
                    cantidades);
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