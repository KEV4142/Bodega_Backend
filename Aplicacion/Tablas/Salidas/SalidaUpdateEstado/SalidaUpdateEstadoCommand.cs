using System.Net;
using Aplicacion.Core;
using Aplicacion.Interface;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Modelo.Entidades;
using Persistencia;

namespace Aplicacion.Tablas.Salidas.SalidaUpdateEstado;
public class SalidaUpdateEstadoCommand
{
    public record SalidaUpdateEstadoCommandRequest(SalidaUpdateEstadoRequest salidaUpdateEstadoRequest, int SalidaID) : IRequest<Result<int>>;

    internal class SalidaUpdateEstadoCommandHandler : IRequestHandler<SalidaUpdateEstadoCommandRequest, Result<int>>
    {
        private readonly BackendContext _backendContext;
        private readonly UserManager<Usuario> _userManager;
        private readonly IUsuarioService _usuarioService;

        public SalidaUpdateEstadoCommandHandler(BackendContext context, UserManager<Usuario> userManager, IUsuarioService usuarioService)
        {
            _backendContext = context;
            _userManager = userManager;
            _usuarioService = usuarioService;
        }
        public async Task<Result<int>> Handle(
            SalidaUpdateEstadoCommandRequest request,
            CancellationToken cancellationToken
        )
        {
            var usuarioResult = await _usuarioService.ObtenerUsuarioActualAsync();
            if (!usuarioResult.IsSuccess)
                {return Result<int>.Failure(usuarioResult.Error!, usuarioResult.StatusCode);}
            var usuario = usuarioResult.Value!;

            var salidaID = request.SalidaID;
            var salida = await _backendContext.SalidaEncs!.FirstOrDefaultAsync(x => x.SalidaID == salidaID);

            if (salida is null)
            {
                return Result<int>.Failure("La Orden de Salida no existe.", HttpStatusCode.NotFound);
            }
            if (salida.Estado.Equals("R", StringComparison.OrdinalIgnoreCase))
            {
                return Result<int>.Failure("La Orden de Salida ya ha sido recibida.", HttpStatusCode.BadRequest);
            }
            salida.Estado = request.salidaUpdateEstadoRequest.Estado!.ToUpper();
            salida.FechaRecibido = DateTime.Now;
            salida.UsuarioRecibe = usuario.Id;
            
            try
            {
                using var transaction = await _backendContext.Database.BeginTransactionAsync(cancellationToken);

                await _backendContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Result<int>.Failure("Errores en la actualizacion del estado de la Orden Salida.", HttpStatusCode.BadRequest);
            }
            return Result<int>.Success(salida.SalidaID);
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