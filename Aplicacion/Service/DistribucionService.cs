using Aplicacion.Interface;
using Aplicacion.Tablas.Salidas.SalidaCreate;
using Aplicacion.Tablas.Salidas.SalidasResponse;

namespace Aplicacion.Service;

public class DistribucionService : IDistribucionService
{
    private readonly ILoteService _loteService;
    private readonly IDistribuidorLotes _distribuidorLotes;

    public DistribucionService(ILoteService loteService, IDistribuidorLotes distribuidorLotes)
    {
        _loteService = loteService;
        _distribuidorLotes = distribuidorLotes;
    }

    public async Task<DistribucionResultado> ObtenerDistribucionAsync(List<SalidaDetRequest> detalles, CancellationToken cancellationToken)
    {
        var loteListaID = detalles.Select(linea => linea.LoteID).ToList();
        var lotesDetalle = await _loteService.ObtenerLotesPorIDLista(loteListaID,cancellationToken);

        var productoCantidadAgrupado = detalles
            .Join(lotesDetalle, detalle => detalle.LoteID, lote => lote.LoteID, (detalle, lote) =>
                new { ProductoID = lote.ProductoID, Cantidad = detalle.Cantidad })
            .GroupBy(linea => linea.ProductoID)
            .Select(grupo => new
            {
                ProductoID = grupo.Key,
                TotalCantidad = grupo.Sum(linea => linea.Cantidad)
            })
            .ToList();
        var lotesValidos = new List<LoteCantidadListado>();

        foreach (var linea in productoCantidadAgrupado)
        {
            var productosListado = await _loteService.ObtenerLotesDisponiblesParaProducto(linea.ProductoID,cancellationToken);

            var seleccionados = _distribuidorLotes.Distribuir(
                productosListado,
                linea.TotalCantidad,
                l => l.Cantidad,
                (l, nuevaCantidad) => l.Cantidad = nuevaCantidad
            );

            lotesValidos.AddRange(seleccionados);
        }

        return new DistribucionResultado
        {
            LotesValidos = lotesValidos,
            LotesDetalle = lotesDetalle
        };
    }
}
