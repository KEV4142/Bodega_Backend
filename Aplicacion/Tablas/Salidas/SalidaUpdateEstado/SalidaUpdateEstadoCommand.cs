using System.Net;
using Aplicacion.Core;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Modelo.Entidades;
using Persistencia;
using Seguridad.Interfaces;

namespace Aplicacion.Tablas.Salidas.SalidaUpdateEstado;
public class SalidaUpdateEstadoCommand
{
    public record SalidaUpdateEstadoCommandRequest(SalidaUpdateEstadoRequest salidaUpdateEstadoRequest, int SalidaID) : IRequest<Result<int>>;

    internal class SalidaUpdateEstadoCommandHandler : IRequestHandler<SalidaUpdateEstadoCommandRequest, Result<int>>
    {
        private readonly BackendContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly IUserAccessor _user;

        public SalidaUpdateEstadoCommandHandler(BackendContext context, UserManager<Usuario> userManager, IUserAccessor user)
        {
            _context = context;
            _userManager = userManager;
            _user = user;
        }
        public async Task<Result<int>> Handle(
            SalidaUpdateEstadoCommandRequest request, 
            CancellationToken cancellationToken
        )
        {
            var usuarioID = _user.GetUserId();
            if (string.IsNullOrEmpty(usuarioID))
            {
                return Result<int>.Failure("No se encontró el UsuarioID en la Autorizacion.", HttpStatusCode.Unauthorized);
            }

            var salidaID = request.SalidaID;
            var salida = await _context.SalidaEncs!.FirstOrDefaultAsync(x => x.SalidaID == salidaID);
            
            if (salida is null)
            {
                return Result<int>.Failure("La Orden de Salida no existe.", HttpStatusCode.NotFound);
            }

            salida.Estado = request.salidaUpdateEstadoRequest.Estado!.ToUpper();
            salida.FechaRecibido = DateTime.Now;
            salida.UsuarioRecibe = usuarioID;

            _context.Entry(salida).State = EntityState.Modified;
            var resultado = await _context.SaveChangesAsync() > 0;

            return resultado 
                        ? Result<int>.Success(salida.SalidaID)
                        : Result<int>.Failure("Errores en la actualizacion del estado de la Orden Salida.", HttpStatusCode.BadRequest);
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