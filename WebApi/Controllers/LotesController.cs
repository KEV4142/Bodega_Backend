using System.Net;
using Aplicacion.Tablas.Lotes.DTOLotes;
using Aplicacion.Tablas.Lotes.GetLotesSalida;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modelo.Custom;
using static Aplicacion.Tablas.Lotes.GetLotesSalida.GetLotesSalidaQuery;

namespace WebApi.Controllers;

[ApiController]
[Route("api/lotes")]
public class LotesController : ControllerBase
{
    private readonly ISender _sender;
    public LotesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Brinda informacion de los Lotes de Inventario ordenados por fecha de vencimiento.
    /// </summary>
    /// <param name="request">Parametros claves (ProductoID y Cantidad).</param>
    /// <param name="cancellationToken">Token de cancelacion por tiempo de espera.</param>
    /// <returns>Una colección de LoteCompletoResponse.</returns>
    [Authorize(Roles = CustomRoles.ADMINBODEGA)]
    [HttpPost("disponible")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult<LoteCompletoResponse>> Disponible(
        [FromBody] GetLotesSalidaRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new GetLotesSalidaQueryRequest(request);
        var resultado = await _sender.Send(command, cancellationToken);
        return resultado.IsSuccess ? Ok(resultado.Value) : StatusCode((int)resultado.StatusCode, resultado);
    }
}