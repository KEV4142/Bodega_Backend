using Aplicacion.Tablas.Lotes.DTOLotes;
using Aplicacion.Tablas.Salidas.SalidasResponse;
using Modelo.Entidades;

namespace Aplicacion.Interface;

public interface ILoteService
{
    Task<List<LoteCompletoResponse>> ObtenerLotesDisponiblesOrdenados(int productoID);
    Task<List<Lote>> ObtenerLotesPorIDLista(List<int> loteIds, CancellationToken cancellationToken);
    Task<List<LoteCantidadListado>> ObtenerLotesDisponiblesParaProducto(int productoID, CancellationToken cancellationToken);
}
