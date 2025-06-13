using Aplicacion.Core;
using Aplicacion.Interface;
using FluentValidation;
using MediatR;

namespace Aplicacion.Tablas.Salidas.SalidaUpdateEstado;

public class SalidaUpdateEstadoCommand
{
    public record SalidaUpdateEstadoCommandRequest(SalidaUpdateEstadoRequest salidaUpdateEstadoRequest, int SalidaID) : IRequest<Result<int>>;

    internal class SalidaUpdateEstadoCommandHandler : IRequestHandler<SalidaUpdateEstadoCommandRequest, Result<int>>
    {
        private readonly ISalidaService _salidaService;
        private readonly IUsuarioService _usuarioService;

        public SalidaUpdateEstadoCommandHandler(ISalidaService salidaService, IUsuarioService usuarioService)
        {
            _salidaService = salidaService;
            _usuarioService = usuarioService;
        }
        public async Task<Result<int>> Handle(
            SalidaUpdateEstadoCommandRequest request,
            CancellationToken cancellationToken
        )
        {
            var usuarioResult = await _usuarioService.ObtenerUsuarioActualAsync(cancellationToken);
            if (!usuarioResult.IsSuccess)
            { return Result<int>.Failure(usuarioResult.Error!, usuarioResult.StatusCode); }
            var usuario = usuarioResult.Value!;

            var salidaID = request.SalidaID;

            var salida = await _salidaService.ObtenerSalidaPorID(salidaID, cancellationToken);
            if (!salida.IsSuccess)
                return Result<int>.Failure(salida.Error!, salida.StatusCode);

            var actualizarResultado = await _salidaService.CambiarEstadoSalida(
                                        salida.Value!,
                                        request.salidaUpdateEstadoRequest.Estado!.ToUpper(),
                                        usuario.Id,
                                        cancellationToken
                                    );
            if (!actualizarResultado.IsSuccess)
                return Result<int>.Failure(actualizarResultado.Error!, actualizarResultado.StatusCode);

            return actualizarResultado;
        }
    }

    public class SalidaUpdateEstadoCommandRequestValidator : AbstractValidator<SalidaUpdateEstadoCommandRequest>
    {
        public SalidaUpdateEstadoCommandRequestValidator()
        {
            RuleFor(x => x.salidaUpdateEstadoRequest).SetValidator(new SalidaUpdateEstadoValidator());
            RuleFor(x => x.SalidaID).NotNull();
        }
    }
}