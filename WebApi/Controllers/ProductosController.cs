using System.Net;
using Aplicacion.Tablas.Productos.DTOProductos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Modelo.Custom;
using static Aplicacion.Tablas.Productos.GetProducto.GetProductoQuery;
using static Aplicacion.Tablas.Productos.GetProductosActivos.GetProductosActivos;

namespace WebApi.Controllers;

[ApiController]
[Route("api/productos")]
public class ProductosController : ControllerBase
{
    private readonly ISender _sender;

    public ProductosController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Brinda informacion del Producto solicitado por el parametro ID.
    /// </summary>
    /// <param name="id">Parametros claves (id).</param>
    /// <param name="cancellationToken">Token de cancelacion por tiempo de espera.</param>
    /// <returns>Un DTO ProductoResponse.</returns>
    [Authorize(Roles = CustomRoles.ADMINBODEGA)]
    [HttpGet("{id}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [OutputCache(Duration = 60)]
    public async Task<ActionResult<ProductoResponse>> ProductoGet(
        int id,
        CancellationToken cancellationToken
    )
    {
        var query = new GetProductoQueryRequest { ProductoID = id };
        var resultado = await _sender.Send(query, cancellationToken);
        return resultado.IsSuccess ? Ok(resultado.Value) : StatusCode((int)resultado.StatusCode, resultado);
    }

    /// <summary>
    /// Brinda informacion de los Productos activos en Listado.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelacion por tiempo de espera.</param>
    /// <returns>Un DTO Listado  de ProductoResponse.</returns>
    [Authorize(Roles = CustomRoles.ADMINBODEGA)]
    [HttpGet("activos")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [OutputCache(Duration = 60)]
    public async Task<ActionResult<ProductoResponse>> GetProductosActivos(
        CancellationToken cancellationToken
    )
    {
        var query = new GetProductosActivasQueryRequest();
        var resultado = await _sender.Send(query, cancellationToken);
        return resultado.IsSuccess ? Ok(resultado.Value) : StatusCode((int)resultado.StatusCode, resultado);

    }
}