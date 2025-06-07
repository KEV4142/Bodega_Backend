using System.Net;
using Aplicacion.Core;
using Aplicacion.Tablas.Salidas.GetSalidasPagin;
using Aplicacion.Tablas.Salidas.SalidaCreate;
using Aplicacion.Tablas.Salidas.SalidasResponse;
using Aplicacion.Tablas.Salidas.SalidaUpdateEstado;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modelo.Custom;
using static Aplicacion.Tablas.Salidas.GetSalidasPagin.GetSalidasPaginQuery;
using static Aplicacion.Tablas.Salidas.SalidaCreate.SalidaEncCreateCommand;
using static Aplicacion.Tablas.Salidas.SalidaUpdateEstado.SalidaUpdateEstadoCommand;

namespace WebApi.Controllers;

[ApiController]
[Route("api/salidas")]
public class SalidasController : ControllerBase
{
    private readonly ISender _sender;
    public SalidasController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Registro de Salidas de Inventario.
    /// </summary>
    /// <param name="request">SucursalID y DetalleSalida(LoteID y Cantidad)</param>
    /// <param name="cancellationToken">Token de cancelacion por tiempo de espera.</param>
    /// <returns>Un valor entero con el numero de la orden de Salida de inventario(SalidaID).</returns>
    [Authorize(Roles = CustomRoles.ADMINBODEGA)]
    [HttpPost("ingreso")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult<int>> Salida(
        [FromBody] SalidaEncCreateRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new SalidaEncCreateCommandRequest(request);
        var resultado = await _sender.Send(command, cancellationToken);
        return resultado.IsSuccess ? Ok(resultado.Value) : StatusCode((int)resultado.StatusCode, resultado);
    }

    /// <summary>
    /// Get de Paginacion de las Salidas de Inventario.
    /// </summary>
    /// <param name="request">FromQuery campos obligatorios(PageNumber y PageSize).</param>
    /// <param name="cancellationToken">Token de cancelacion por tiempo de espera.</param>
    /// <returns>Un SalidaListaResponsePagedList(currentPage,PageSize,totalPage y la coleccion de registros de Salidas de inventario).</returns>
    [Authorize(Roles = CustomRoles.ADMINBODEGA)]
    [HttpGet("paginacion")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult<PagedList<SalidaListaResponse>>> PaginationSalidas(
        [FromQuery] GetSalidasPaginRequest request,
        CancellationToken cancellationToken
    )
    {

        var query = new GetSalidasPaginQueryRequest { SalidasPaginRequest = request };
        var resultado = await _sender.Send(query, cancellationToken);

        return resultado.IsSuccess ? Ok(resultado.Value) : StatusCode((int)resultado.StatusCode, resultado);
    }

    /// <summary>
    /// Recepción de Salidas de Inventario.
    /// </summary>
    /// <param name="request">Campo Estado, ya el usuario se obtiene por autenticación.</param>
    /// <param name="id">ID de la orden de Salida(SalidaID).</param>
    /// <param name="cancellationToken">Token de cancelacion por tiempo de espera.</param>
    /// <returns>Un valor entero con el numero de la orden de Salida de inventario(SalidaID).</returns>
    [Authorize(Roles = $"{CustomRoles.ADMINBODEGA},{CustomRoles.CLIENT}")]
    [HttpPut("estado/{id}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult<Result<int>>> SalidaUpdateEstado(
        [FromBody] SalidaUpdateEstadoRequest request,
        int id,
        CancellationToken cancellationToken
    )
    {
        var command = new SalidaUpdateEstadoCommandRequest(request, id);
        var resultado = await _sender.Send(command, cancellationToken);
        return resultado.IsSuccess ? Ok(resultado) : StatusCode((int)resultado.StatusCode, resultado);
    }



}