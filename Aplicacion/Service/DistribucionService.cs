using Aplicacion.Core;
using Aplicacion.Interface;
using Aplicacion.Tablas.Salidas.SalidaCreate;
using Aplicacion.Tablas.Salidas.SalidasResponse;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Persistencia;

namespace Aplicacion.Service;

public class DistribucionService : IDistribucionService
{
    private readonly BackendContext _backendContext;
    private readonly IMapper _mapper;
    private readonly IDistribuidorLotes _distribuidorLotes;

    public DistribucionService(BackendContext context, IMapper mapper, IDistribuidorLotes distribuidorLotes)
    {
        _backendContext = context;
        _mapper = mapper;
        _distribuidorLotes = distribuidorLotes;
    }

    public async Task<DistribucionResultado> ObtenerDistribucionAsync(List<SalidaDetRequest> detalles, CancellationToken cancellationToken)
    {
        var loteListaID = detalles.Select(linea => linea.LoteID).ToList();
        var lotesDetalle = await _backendContext.Lotes!.Where(lote => loteListaID.Contains(lote.LoteID)).ToListAsync(cancellationToken);
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
            var productosListado = await _backendContext.Lotes!
                .Where(l => l.ProductoID == linea.ProductoID && l.Cantidad > 0)
                .OrderBy(l => l.FechaVencimiento)
                .ProjectTo<LoteCantidadListado>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

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
