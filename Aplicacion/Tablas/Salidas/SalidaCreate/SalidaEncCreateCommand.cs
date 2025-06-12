using System.Net;
using Aplicacion.Core;
using Aplicacion.Interface;
using FluentValidation;
using MediatR;
using Modelo.Entidades;

namespace Aplicacion.Tablas.Salidas.SalidaCreate;

public class SalidaEncCreateCommand
{
    public record SalidaEncCreateCommandRequest(SalidaEncCreateRequest salidaEncCreateRequest) : IRequest<Result<int>>;

    internal class SalidaEncCreateCommandHandler : IRequestHandler<SalidaEncCreateCommandRequest, Result<int>>
    {
        private readonly ISalidaService _salidaService;
        private readonly IDistribucionService _distribucionService;
        private readonly IRestriccionSalidaService _restriccionSalidaService;
        private readonly IUsuarioService _usuarioService;
        private readonly ISucursalService _sucursalService;
        private readonly ISalidaDetListBuilder _salidaDetListBuilder;

        public SalidaEncCreateCommandHandler(ISalidaService salidaService, IDistribucionService distribucionService, IRestriccionSalidaService restriccionSalidaService, IUsuarioService usuarioService, ISucursalService sucursalService, ISalidaDetListBuilder salidaDetListBuilder)
        {
            _salidaService = salidaService;
            _distribucionService = distribucionService;
            _restriccionSalidaService = restriccionSalidaService;
            _usuarioService = usuarioService;
            _sucursalService = sucursalService;
            _salidaDetListBuilder = salidaDetListBuilder;
        }

        public async Task<Result<int>> Handle(SalidaEncCreateCommandRequest request, CancellationToken cancellationToken)
        {
            var usuarioResult = await _usuarioService.ObtenerUsuarioActualAsync();
            if (!usuarioResult.IsSuccess)
            { return Result<int>.Failure(usuarioResult.Error!, usuarioResult.StatusCode); }
            var usuario = usuarioResult.Value!;

            var salidaEnc = new SalidaEnc
            {
                Fecha = DateTime.Now,
                SucursalID = request.salidaEncCreateRequest.SucursalID,
                UsuarioID = usuario.Id
            };
            var sucursalResultado = await _sucursalService.ObtenerSucursalPorID(request.salidaEncCreateRequest.SucursalID);
            if (!sucursalResultado.IsSuccess)
            { return Result<int>.Failure(sucursalResultado.Error!, HttpStatusCode.NotFound); }

            salidaEnc.Sucursales = sucursalResultado.Value!;

            var distribucion = await _distribucionService.ObtenerDistribucionAsync(request.salidaEncCreateRequest.SalidasDetalle, cancellationToken);
            var lotesDetalle = distribucion.LotesDetalle;
            var lotesValidos = distribucion.LotesValidos;

            decimal sumaDetalle = 0;

            var resultadoDetalle = _salidaDetListBuilder.Construir(
                                        request.salidaEncCreateRequest.SalidasDetalle,
                                        lotesDetalle,
                                        lotesValidos,
                                        salidaEnc
                                    );

            if (!resultadoDetalle.IsSuccess)
                return Result<int>.Failure(resultadoDetalle.Error!, resultadoDetalle.StatusCode);

            salidaEnc.SalidaDets = resultadoDetalle.Value!.SalidasDetalles;
            sumaDetalle = resultadoDetalle.Value.Total;


            var restriccionResult = await _restriccionSalidaService.ValidarLimiteSucursal(salidaEnc.SucursalID, sumaDetalle, cancellationToken);
            if (!restriccionResult.IsSuccess)
            {
                return Result<int>.Failure(restriccionResult.Error!, HttpStatusCode.BadRequest);
            }

            var insertarResultado = await _salidaService.RegistrarSalida(salidaEnc, cancellationToken);

            return insertarResultado;
        }
    }


    public class SalidaEncCreateCommandRequestValidator : AbstractValidator<SalidaEncCreateCommandRequest>
    {
        public SalidaEncCreateCommandRequestValidator()
        {
            RuleFor(x => x.salidaEncCreateRequest).SetValidator(new SalidaCreateValidator());
        }
    }
}