using Modelo.Entidades;

namespace Modelo.Interfaces;

public interface ILoteRepository
{
    Task<List<Lote>> ObtenerLotesDisponiblesOrdenadosAsync(int productoID);

}
