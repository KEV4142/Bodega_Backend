using System.Net;
using Aplicacion.Core;
using Aplicacion.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Modelo.Entidades;
using Seguridad.Interfaces;

namespace Aplicacion.Service;

public class UsuarioService : IUsuarioService
{
    private readonly IUserAccessor _userAccessor;
    private readonly UserManager<Usuario> _userManager;
    public UsuarioService(IUserAccessor userAccessor, UserManager<Usuario> userManager)
    {
        _userAccessor = userAccessor;
        _userManager = userManager;
    }

    public async Task<Result<Usuario>> ObtenerUsuarioActualAsync(CancellationToken cancellationToken)
    {
        var usuarioID = _userAccessor.GetUserId();
        if (string.IsNullOrEmpty(usuarioID))
            {return Result<Usuario>.Failure("No se encontró el UsuarioID en la Autorización.", HttpStatusCode.Unauthorized);}

        var usuario = await _userManager.Users!.FirstOrDefaultAsync(x => x.Id == usuarioID, cancellationToken);
        if (usuario is null)
            return Result<Usuario>.Failure("No se encontró el Usuario.", HttpStatusCode.NotFound);

        return Result<Usuario>.Success(usuario);
    }
}