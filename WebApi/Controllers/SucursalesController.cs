using System.Net;
using Aplicacion.Tablas.Sucursales.DTOSucursales;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Modelo.Custom;
using static Aplicacion.Tablas.Sucursales.GetSucursalesActivas.GetSucursalesActivas;

namespace WebApi.Controllers;

[ApiController]
[Route("api/sucursales")]
public class SucursalesController:ControllerBase
{
    private readonly ISender _sender;
    public SucursalesController(ISender sender)
    {
        _sender = sender;
    }

    [Authorize(Roles = CustomRoles.ADMINBODEGA)]
    [HttpGet("activos")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [OutputCache(Duration = 60)]
    public async Task<ActionResult<SucursalResponse>> GetSucursalesActivos(
        CancellationToken cancellationToken
    )
    {
        var query = new GetSucursalesActivasQueryRequest();
        var resultado = await _sender.Send(query, cancellationToken);
        return resultado.IsSuccess ? Ok(resultado.Value) : StatusCode((int)resultado.StatusCode, resultado);

    }
}