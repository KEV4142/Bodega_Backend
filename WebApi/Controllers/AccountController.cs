using System.Net;
using Aplicacion.Tablas.Accounts;
using Aplicacion.Tablas.Accounts.GetCurrentUser;
using Aplicacion.Tablas.Accounts.Login;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seguridad.Interfaces;
using static Aplicacion.Tablas.Accounts.GetCurrentUser.GetCurrentUserQuery;
using static Aplicacion.Tablas.Accounts.Login.LoginCommand;

namespace WebApi.Controllers;

[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IUserAccessor _user;
    public AccountController(ISender sender, IUserAccessor user)
    {
        _sender = sender;
        _user = user;
    }


    /// <summary>
    /// Inicia sesión con credenciales de usuario.
    /// </summary>
    /// <param name="request">Credenciales del usuario(correo electronico y contraseña)</param>
    /// <param name="cancellationToken">Token de cancelacion por tiempo de espera.</param>
    /// <returns>Token JWT si las credenciales son válidas junto a Profile</returns>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult<Profile>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new LoginCommandRequest(request);
        var resultado = await _sender.Send(command, cancellationToken);
        return resultado.IsSuccess ? Ok(resultado.Value) : Unauthorized(resultado);
    }

    /// <summary>
    /// Devuelve la información del usuario autenticado.
    /// </summary>
    /// <remarks>
    /// No se requiere ningún parámetro.
    /// </remarks>
    /// <returns>Token JWT si las credenciales son válidas junto a Profile</returns>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult<Profile>> Me(CancellationToken cancellationToken)
    {
        var email = _user.GetEmail();
        var request = new GetCurrentUserRequest {Email = email};
        
        var query = new GetCurrentUserQueryRequest(request);
        var resultado =  await _sender.Send(query, cancellationToken);
        return resultado.IsSuccess ? Ok(resultado.Value) : StatusCode((int)resultado.StatusCode, resultado);
    }
}
