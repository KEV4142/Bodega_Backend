using Aplicacion.Tablas.Lotes.DTOLotes;

namespace Aplicacion.Interface;

public interface ILoteService
{
    Task<List<LoteCompletoResponse>> ObtenerLotesDisponiblesOrdenados(int productoID);
}
